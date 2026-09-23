using System.Text.Json;
using Umbraco.Cms.Core;

namespace HeadlessDemo.Cms.Seeding;

/// <summary>One block in a Block Grid: an element type alias, its column span and its property values.</summary>
public sealed record GridBlock(string ContentTypeAlias, int ColumnSpan, IReadOnlyDictionary<string, object?> Values)
{
    /// <summary>Child blocks placed in one of the block's areas (used by the card row).</summary>
    public GridArea? Area { get; init; }
}

public sealed record GridArea(Guid Key, IReadOnlyList<GridBlock> Items);

/// <summary>
/// Serializes blocks to the Block Grid and Block List storage formats used since Umbraco 15:
/// layout (with column spans and areas for the grid) + contentData + settingsData + expose.
/// </summary>
public static class BlockGridValue
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static string Serialize(IEnumerable<GridBlock> blocks, Func<string, Guid> elementTypeKey)
    {
        var contentData = new List<object>();
        var expose = new List<object>();

        object Layout(GridBlock block)
        {
            Guid key = Guid.NewGuid();
            contentData.Add(ContentData(key, block, elementTypeKey));
            expose.Add(Expose(key));

            object[] areas = block.Area is { } area
                ? [new { key = area.Key, items = area.Items.Select(Layout).ToArray() }]
                : [];

            return new { contentKey = key, settingsKey = (Guid?)null, columnSpan = block.ColumnSpan, rowSpan = 1, areas };
        }

        object[] layout = blocks.Select(Layout).ToArray();

        return JsonSerializer.Serialize(
            new
            {
                layout = new Dictionary<string, object> { [Constants.PropertyEditors.Aliases.BlockGrid] = layout },
                contentData,
                settingsData = Array.Empty<object>(),
                expose,
            },
            JsonOptions);
    }

    /// <summary>
    /// Block List value, as an object so it can nest inside a block's property values.
    /// Column spans and areas on the items are ignored; a list has neither.
    /// </summary>
    public static object BlockList(IEnumerable<GridBlock> items, Func<string, Guid> elementTypeKey)
    {
        var contentData = new List<object>();
        var expose = new List<object>();
        var layout = new List<object>();

        foreach (GridBlock item in items)
        {
            Guid key = Guid.NewGuid();
            contentData.Add(ContentData(key, item, elementTypeKey));
            expose.Add(Expose(key));
            layout.Add(new { contentKey = key, settingsKey = (Guid?)null });
        }

        return new
        {
            layout = new Dictionary<string, object> { [Constants.PropertyEditors.Aliases.BlockList] = layout },
            contentData,
            settingsData = Array.Empty<object>(),
            expose,
        };
    }

    private static object ContentData(Guid key, GridBlock block, Func<string, Guid> elementTypeKey) => new
    {
        key,
        contentTypeKey = elementTypeKey(block.ContentTypeAlias),
        values = block.Values.Select(v => new { alias = v.Key, value = v.Value, culture = (string?)null, segment = (string?)null }),
    };

    private static object Expose(Guid key) => new { contentKey = key, culture = (string?)null, segment = (string?)null };

    /// <summary>Rich text editor value (markup plus embedded blocks, none here).</summary>
    public static object RichText(string markup) => new { markup, blocks = (object?)null };

    /// <summary>Media Picker value holding a single image.</summary>
    public static object[] Image(Guid mediaKey) =>
    [
        new
        {
            key = Guid.NewGuid(),
            mediaKey,
            mediaTypeAlias = Constants.Conventions.MediaTypes.Image,
            crops = Array.Empty<object>(),
            focalPoint = (object?)null,
        },
    ];

    /// <summary>Content Picker value (a document UDI).</summary>
    public static string Document(Guid key) => Udi.Create(Constants.UdiEntityType.Document, key).ToString();

    public static string ImageJson(Guid mediaKey) => JsonSerializer.Serialize(Image(mediaKey), JsonOptions);
}
