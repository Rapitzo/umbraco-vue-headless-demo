using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace HeadlessDemo.Cms.Seeding;

/// <summary>
/// Creates the content model: a page composition, six block element types, a Block Grid
/// data type and five document types. Every item is get-or-create, so running it again
/// against an existing database changes nothing.
/// </summary>
public sealed class SchemaSeeder
{
    /// <summary>Key of the "cards" area inside the card row block. Content seeding references it.</summary>
    public static readonly Guid CardsAreaKey = new("6f1d6a52-2c0e-4b52-9a51-3f3c1b7d9e10");

    private const string BlockGridName = "Page Blocks";

    private readonly IContentTypeService _contentTypeService;
    private readonly IMediaTypeService _mediaTypeService;
    private readonly IDataTypeService _dataTypeService;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IConfigurationEditorJsonSerializer _configSerializer;
    private readonly IShortStringHelper _shortStringHelper;

    public SchemaSeeder(
        IContentTypeService contentTypeService,
        IMediaTypeService mediaTypeService,
        IDataTypeService dataTypeService,
        PropertyEditorCollection propertyEditors,
        IConfigurationEditorJsonSerializer configSerializer,
        IShortStringHelper shortStringHelper)
    {
        _contentTypeService = contentTypeService;
        _mediaTypeService = mediaTypeService;
        _dataTypeService = dataTypeService;
        _propertyEditors = propertyEditors;
        _configSerializer = configSerializer;
        _shortStringHelper = shortStringHelper;
    }

    public async Task EnsureAsync()
    {
        IDataType textstring = await GetDataTypeAsync(Constants.DataTypes.Guids.TextstringGuid);
        IDataType textarea = await GetDataTypeAsync(Constants.DataTypes.Guids.TextareaGuid);
        IDataType richText = await GetDataTypeAsync(Constants.DataTypes.Guids.RichtextEditorGuid);
        IDataType image = await GetDataTypeAsync(Constants.DataTypes.Guids.MediaPicker3SingleImageGuid);
        IDataType contentPicker = await GetDataTypeAsync(Constants.DataTypes.Guids.ContentPickerGuid);
        IDataType checkbox = await GetDataTypeAsync(Constants.DataTypes.Guids.CheckboxGuid);
        IDataType datePicker = await GetDataTypeAsync(Constants.DataTypes.Guids.DatePickerGuid);

        await EnsureImageAltTextAsync(textstring);

        // Block element types
        IContentType hero = await EnsureTypeAsync("heroBlock", "Hero", "icon-picture", isElement: true, type =>
        {
            AddProperty(type, textstring, "heading", "Heading", mandatory: true);
            AddProperty(type, textarea, "text", "Text");
            AddProperty(type, image, "image", "Image");
            AddProperty(type, textstring, "ctaLabel", "Button label");
            AddProperty(type, contentPicker, "ctaLink", "Button links to");
        });
        IContentType richTextBlock = await EnsureTypeAsync("richTextBlock", "Rich text", "icon-paragraph", isElement: true, type =>
            AddProperty(type, richText, "text", "Text", mandatory: true));
        IContentType imageBlock = await EnsureTypeAsync("imageBlock", "Image", "icon-picture", isElement: true, type =>
        {
            AddProperty(type, image, "image", "Image", mandatory: true);
            AddProperty(type, textstring, "caption", "Caption");
        });
        IContentType quote = await EnsureTypeAsync("quoteBlock", "Quote", "icon-quote", isElement: true, type =>
        {
            AddProperty(type, textarea, "quote", "Quote", mandatory: true);
            AddProperty(type, textstring, "attribution", "Attribution");
        });
        IContentType card = await EnsureTypeAsync("cardBlock", "Card", "icon-document", isElement: true, type =>
        {
            AddProperty(type, textstring, "title", "Title", mandatory: true);
            AddProperty(type, textarea, "text", "Text");
            AddProperty(type, image, "image", "Image");
            AddProperty(type, contentPicker, "link", "Links to");
        });
        IContentType cardRow = await EnsureTypeAsync("cardRowBlock", "Card row", "icon-thumbnails-small", isElement: true, type =>
            AddProperty(type, textstring, "heading", "Heading"));

        IDataType blocks = await EnsureBlockGridAsync(hero, richTextBlock, imageBlock, quote, cardRow, card);

        // Shared page properties, composed into every document type
        IContentType pageBase = await EnsureTypeAsync("pageBase", "Page base", "icon-settings", isElement: false, type =>
        {
            AddProperty(type, textarea, "metaDescription", "Meta description", group: "seo", groupName: "SEO");
            AddProperty(type, checkbox, "hideFromNavigation", "Hide from navigation", group: "seo", groupName: "SEO");
        });

        IContentType newsItem = await EnsureTypeAsync("newsItem", "News item", "icon-newspaper", isElement: false, type =>
        {
            type.AddContentType(pageBase);
            AddProperty(type, datePicker, "publishDate", "Publish date", mandatory: true);
            AddProperty(type, textarea, "teaser", "Teaser", mandatory: true);
            AddProperty(type, image, "image", "Image");
            AddProperty(type, blocks, "blocks", "Body");
        });
        IContentType newsList = await EnsureTypeAsync("newsList", "News list", "icon-bulleted-list", isElement: false, type =>
        {
            type.AddContentType(pageBase);
            AddProperty(type, textarea, "intro", "Intro");
            type.AllowedContentTypes = [new ContentTypeSort(newsItem.Key, 0, newsItem.Alias)];
        });
        IContentType contentPage = await EnsureTypeAsync("contentPage", "Content page", "icon-document", isElement: false, type =>
        {
            type.AddContentType(pageBase);
            AddProperty(type, textarea, "intro", "Intro");
            AddProperty(type, blocks, "blocks", "Blocks");
        });
        IContentType contact = await EnsureTypeAsync("contactPage", "Contact page", "icon-message", isElement: false, type =>
        {
            type.AddContentType(pageBase);
            AddProperty(type, textarea, "intro", "Intro");
            AddProperty(type, image, "image", "Image");
            AddProperty(type, textarea, "formIntro", "Form intro");
        });

        await EnsureTypeAsync("home", "Home", "icon-home", isElement: false, type =>
        {
            type.AllowedAsRoot = true;
            type.AddContentType(pageBase);
            AddProperty(type, blocks, "blocks", "Blocks");

            // Site settings live on the root node: one site, one place to edit them.
            const string settings = "siteSettings";
            AddProperty(type, textstring, "siteName", "Site name", mandatory: true, group: settings, groupName: "Site settings");
            AddProperty(type, textstring, "logoText", "Logo text", group: settings, groupName: "Site settings");
            AddProperty(type, textstring, "tagline", "Tagline", group: settings, groupName: "Site settings");
            AddProperty(type, textarea, "address", "Address", group: settings, groupName: "Site settings");
            AddProperty(type, textarea, "openingHours", "Opening hours (one line per day range)", group: settings, groupName: "Site settings");
            AddProperty(type, textstring, "phone", "Phone", group: settings, groupName: "Site settings");
            AddProperty(type, textstring, "email", "Email", group: settings, groupName: "Site settings");
            AddProperty(type, textarea, "footerNote", "Footer note", group: settings, groupName: "Site settings");

            type.AllowedContentTypes =
            [
                new ContentTypeSort(contentPage.Key, 0, contentPage.Alias),
                new ContentTypeSort(newsList.Key, 1, newsList.Alias),
                new ContentTypeSort(contact.Key, 2, contact.Alias),
            ];
        });
    }

    private async Task<IDataType> GetDataTypeAsync(Guid key) =>
        await _dataTypeService.GetAsync(key)
        ?? throw new InvalidOperationException($"Built-in data type {key} not found.");

    /// <summary>The stock Image media type has no alt text. Editors need one for accessible output.</summary>
    private async Task EnsureImageAltTextAsync(IDataType textstring)
    {
        IMediaType mediaType = _mediaTypeService.Get(Constants.Conventions.MediaTypes.Image)
            ?? throw new InvalidOperationException("Image media type not found.");
        if (mediaType.PropertyTypeExists("altText"))
        {
            return;
        }

        mediaType.AddPropertyType(
            new PropertyType(_shortStringHelper, textstring, "altText") { Name = "Alt text", Description = "Describe the image for screen readers." },
            "image",
            "Image");
        var result = await _mediaTypeService.UpdateAsync(mediaType, Constants.Security.SuperUserKey);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Could not add alt text to the Image media type: {result.Result}");
        }
    }

    private async Task<IContentType> EnsureTypeAsync(string alias, string name, string icon, bool isElement, Action<ContentType> configure)
    {
        if (_contentTypeService.Get(alias) is { } existing)
        {
            return existing;
        }

        var type = new ContentType(_shortStringHelper, Constants.System.Root)
        {
            Alias = alias,
            Name = name,
            Icon = icon,
            IsElement = isElement,
        };
        configure(type);

        var result = await _contentTypeService.CreateAsync(type, Constants.Security.SuperUserKey);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Could not create content type '{alias}': {result.Result}");
        }

        return type;
    }

    private void AddProperty(
        ContentType type,
        IDataType dataType,
        string alias,
        string name,
        bool mandatory = false,
        string group = "content",
        string groupName = "Content") =>
        type.AddPropertyType(new PropertyType(_shortStringHelper, dataType, alias) { Name = name, Mandatory = mandatory }, group, groupName);

    private async Task<IDataType> EnsureBlockGridAsync(
        IContentType hero,
        IContentType richText,
        IContentType image,
        IContentType quote,
        IContentType cardRow,
        IContentType card)
    {
        if (await _dataTypeService.GetAsync(BlockGridName) is { } existing)
        {
            return existing;
        }

        IDataEditor editor = _propertyEditors[Constants.PropertyEditors.Aliases.BlockGrid]
            ?? throw new InvalidOperationException("Block Grid property editor not found.");

        // Stored as the backoffice stores it, so the grid editor shows the same options an editor
        // would get from configuring the data type by hand (column spans, areas, allowed blocks).
        var dataType = new DataType(editor, _configSerializer)
        {
            Name = BlockGridName,
            EditorUiAlias = "Umb.PropertyEditorUi.BlockGrid",
            ConfigurationData = new Dictionary<string, object>
            {
                ["gridColumns"] = 12,
                ["blocks"] = new object[]
                {
                    RootBlock(hero, 12),
                    RootBlock(richText, 12, 8, 6),
                    RootBlock(image, 12, 6),
                    RootBlock(quote, 12, 6),
                    new
                    {
                        contentElementTypeKey = cardRow.Key,
                        allowAtRoot = true,
                        allowInAreas = false,
                        columnSpanOptions = Spans(12),
                        areaGridColumns = 12,
                        areas = new[]
                        {
                            new
                            {
                                key = CardsAreaKey,
                                alias = "cards",
                                columnSpan = 12,
                                rowSpan = 1,
                                specifiedAllowance = new[] { new { elementTypeKey = card.Key } },
                            },
                        },
                    },
                    new
                    {
                        contentElementTypeKey = card.Key,
                        allowAtRoot = false,
                        allowInAreas = true,
                        columnSpanOptions = Spans(4, 6, 12),
                    },
                },
            },
        };

        var result = await _dataTypeService.CreateAsync(dataType, Constants.Security.SuperUserKey);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Could not create the Block Grid data type: {result.Status}");
        }

        return result.Result;
    }

    private static object RootBlock(IContentType type, params int[] spans) => new
    {
        contentElementTypeKey = type.Key,
        allowAtRoot = true,
        allowInAreas = false,
        columnSpanOptions = Spans(spans),
    };

    private static object[] Spans(params int[] spans) => spans.Select(span => (object)new { columnSpan = span }).ToArray();
}
