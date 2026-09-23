using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;

namespace HeadlessDemo.Cms.DeliveryApi;

/// <summary>
/// Indexes the news item publish date and adds a matching Delivery API sort option,
/// so the frontend can ask for <c>sort=publishDate:desc</c> instead of relying on tree order.
/// Umbraco discovers index and query handlers automatically.
/// </summary>
public sealed class PublishDateSortHandler : IContentIndexHandler, ISortHandler
{
    private const string FieldName = "publishDate";
    private const string SortPrefix = "publishDate:";

    public IEnumerable<IndexField> GetFields() =>
        [new IndexField { FieldName = FieldName, FieldType = FieldType.Date, VariesByCulture = false }];

    public IEnumerable<IndexFieldValue> GetFieldValues(IContent content, string? culture) =>
        content.GetValue<DateTime?>(FieldName) is { } date
            ? [new IndexFieldValue { FieldName = FieldName, Values = [date] }]
            : [];

    public bool CanHandle(string query) => query.StartsWith(SortPrefix, StringComparison.OrdinalIgnoreCase);

    public SortOption BuildSortOption(string sort) => new()
    {
        FieldName = FieldName,
        Direction = sort[SortPrefix.Length..].StartsWith("asc", StringComparison.OrdinalIgnoreCase)
            ? Direction.Ascending
            : Direction.Descending,
    };
}
