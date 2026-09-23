using System.Text.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace HeadlessDemo.Cms.Seeding;

/// <summary>
/// Creates the demo schema (two element types, a block list, two document types)
/// and publishes a few articles on first boot. Runs once: skips if the "article"
/// document type already exists.
/// </summary>
public sealed class DemoContentSeeder : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private readonly IRuntimeState _runtimeState;
    private readonly IContentTypeService _contentTypeService;
    private readonly IDataTypeService _dataTypeService;
    private readonly IContentService _contentService;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IConfigurationEditorJsonSerializer _configSerializer;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly ILogger<DemoContentSeeder> _logger;

    public DemoContentSeeder(
        IRuntimeState runtimeState,
        IContentTypeService contentTypeService,
        IDataTypeService dataTypeService,
        IContentService contentService,
        PropertyEditorCollection propertyEditors,
        IConfigurationEditorJsonSerializer configSerializer,
        IShortStringHelper shortStringHelper,
        ILogger<DemoContentSeeder> logger)
    {
        _runtimeState = runtimeState;
        _contentTypeService = contentTypeService;
        _dataTypeService = dataTypeService;
        _contentService = contentService;
        _propertyEditors = propertyEditors;
        _configSerializer = configSerializer;
        _shortStringHelper = shortStringHelper;
        _logger = logger;
    }

    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        if (_runtimeState.Level != RuntimeLevel.Run || _contentTypeService.Get("article") is not null)
        {
            return;
        }

        IDataType textstring = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.TextstringGuid)
            ?? throw new InvalidOperationException("Textstring data type not found.");
        IDataType textarea = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.TextareaGuid)
            ?? throw new InvalidOperationException("Textarea data type not found.");

        // Element types used as blocks
        ContentType textBlock = CreateType("textBlock", "Text Block", "icon-text", isElement: true);
        AddProperty(textBlock, textstring, "heading", "Heading");
        AddProperty(textBlock, textarea, "text", "Text");

        ContentType quoteBlock = CreateType("quoteBlock", "Quote Block", "icon-quote", isElement: true);
        AddProperty(quoteBlock, textarea, "quote", "Quote");
        AddProperty(quoteBlock, textstring, "attribution", "Attribution");

        await CreateTypeAsync(textBlock);
        await CreateTypeAsync(quoteBlock);

        // Block list data type allowing both element types
        IDataType blockList = await CreateBlockListAsync("Article Body Blocks", textBlock.Key, quoteBlock.Key);

        // Document types
        ContentType article = CreateType("article", "Article", "icon-article", isElement: false);
        AddProperty(article, textarea, "summary", "Summary");
        AddProperty(article, blockList, "body", "Body");
        await CreateTypeAsync(article);

        ContentType articleList = CreateType("articleList", "Article List", "icon-bulleted-list", isElement: false);
        articleList.AllowedAsRoot = true;
        articleList.AllowedContentTypes = new[] { new ContentTypeSort(article.Key, 0, article.Alias) };
        AddProperty(articleList, textarea, "intro", "Intro");
        await CreateTypeAsync(articleList);

        SeedContent(textBlock, quoteBlock);
        _logger.LogInformation("Demo schema and content seeded.");
    }

    private ContentType CreateType(string alias, string name, string icon, bool isElement) =>
        new(_shortStringHelper, Constants.System.Root)
        {
            Alias = alias,
            Name = name,
            Icon = icon,
            IsElement = isElement,
        };

    private async Task CreateTypeAsync(ContentType type)
    {
        var result = await _contentTypeService.CreateAsync(type, Constants.Security.SuperUserKey);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Could not create document type '{type.Alias}': {result.Result}");
        }
    }

    private void AddProperty(ContentType type, IDataType dataType, string alias, string name) =>
        type.AddPropertyType(new PropertyType(_shortStringHelper, dataType, alias) { Name = name }, "content", "Content");

    private async Task<IDataType> CreateBlockListAsync(string name, params Guid[] elementTypeKeys)
    {
        IDataEditor editor = _propertyEditors[Constants.PropertyEditors.Aliases.BlockList]
            ?? throw new InvalidOperationException("Block List property editor not found.");

        var dataType = new DataType(editor, _configSerializer)
        {
            Name = name,
            EditorUiAlias = "Umb.PropertyEditorUi.BlockList",
            ConfigurationData = new Dictionary<string, object>
            {
                ["blocks"] = elementTypeKeys
                    .Select(key => new BlockListConfiguration.BlockConfiguration { ContentElementTypeKey = key })
                    .ToArray(),
            },
        };

        var result = await _dataTypeService.CreateAsync(dataType, Constants.Security.SuperUserKey);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Could not create block list data type: {result.Status}");
        }

        return result.Result;
    }

    private void SeedContent(IContentType textBlock, IContentType quoteBlock)
    {
        IContent root = _contentService.Create("Insights", Constants.System.Root, "articleList");
        root.SetValue("intro", "Articles authored in Umbraco and rendered by a Vue 3 frontend through the Content Delivery API.");
        _contentService.Save(root);
        _contentService.Publish(root, new[] { "*" });

        var articles = new[]
        {
            (
                Name: "Why go headless with Umbraco",
                Summary: "Editors keep the Umbraco backoffice. The frontend team ships a separate Vue app on its own release cycle.",
                Blocks: new[]
                {
                    Text(textBlock, "One content model, many channels", "The Content Delivery API exposes published content as JSON. Any client can read it: a Vue site, a mobile app or a kiosk screen."),
                    Quote(quoteBlock, "The CMS owns the content and the URLs. The frontend owns the rendering.", "Design principle for this demo"),
                    Text(textBlock, "What stays in Umbraco", "Document types, validation, publishing workflow, permissions and preview remain in the backoffice."),
                }),
            (
                Name: "Rendering block lists in Vue",
                Summary: "Each block element type maps to one Vue component. Unknown blocks are skipped instead of breaking the page.",
                Blocks: new[]
                {
                    Text(textBlock, "A component registry", "The frontend keeps a map from element type alias to component. Adding a block type means adding one entry and one component."),
                    Text(textBlock, "Typed responses", "Block content arrives with its contentType alias, so a TypeScript discriminated union narrows the properties for each block."),
                }),
            (
                Name: "Typing the Delivery API",
                Summary: "A small hand-written client with generic response types keeps API calls in one place.",
                Blocks: new[]
                {
                    Text(textBlock, "One module for HTTP", "Components never call fetch directly. They call functions such as getArticles() and getContentByPath(), which return typed data."),
                    Quote(quoteBlock, "If the schema changes, the compiler points at every place that needs an update.", "Why the types are worth it"),
                }),
        };

        foreach (var (name, summary, blocks) in articles)
        {
            IContent article = _contentService.Create(name, root, "article");
            article.SetValue("summary", summary);
            article.SetValue("body", BlockListValue(blocks));
            _contentService.Save(article);
            _contentService.Publish(article, new[] { "*" });
        }
    }

    private static BlockItem Text(IContentType type, string heading, string text) =>
        new(Guid.NewGuid(), type.Key, new[] { new BlockValue("heading", heading), new BlockValue("text", text) });

    private static BlockItem Quote(IContentType type, string quote, string attribution) =>
        new(Guid.NewGuid(), type.Key, new[] { new BlockValue("quote", quote), new BlockValue("attribution", attribution) });

    // Block List storage format (Umbraco 15+): layout + contentData + settingsData + expose.
    private static string BlockListValue(IReadOnlyCollection<BlockItem> blocks) =>
        JsonSerializer.Serialize(
            new
            {
                layout = new Dictionary<string, object>
                {
                    [Constants.PropertyEditors.Aliases.BlockList] = blocks.Select(b => new { contentKey = b.Key }),
                },
                contentData = blocks,
                settingsData = Array.Empty<object>(),
                expose = blocks.Select(b => new { contentKey = b.Key, culture = (string?)null, segment = (string?)null }),
            },
            JsonOptions);

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private sealed record BlockItem(Guid Key, Guid ContentTypeKey, IReadOnlyList<BlockValue> Values);

    private sealed record BlockValue(string Alias, string Value, string? Culture = null, string? Segment = null);
}
