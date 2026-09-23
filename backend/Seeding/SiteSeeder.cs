using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace HeadlessDemo.Cms.Seeding;

/// <summary>
/// Runs on startup once Umbraco is installed: ensures the content model, imports the photos
/// and, if the content tree is empty, creates and publishes the site. Safe to run on every boot.
/// </summary>
public sealed class SiteSeeder : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
{
    private readonly IRuntimeState _runtimeState;
    private readonly SchemaSeeder _schema;
    private readonly MediaSeeder _media;
    private readonly ContentSeeder _content;
    private readonly ILogger<SiteSeeder> _logger;

    public SiteSeeder(
        IRuntimeState runtimeState,
        SchemaSeeder schema,
        MediaSeeder media,
        ContentSeeder content,
        ILogger<SiteSeeder> logger)
    {
        _runtimeState = runtimeState;
        _schema = schema;
        _media = media;
        _content = content;
        _logger = logger;
    }

    public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
    {
        if (_runtimeState.Level != RuntimeLevel.Run)
        {
            return;
        }

        await _schema.EnsureAsync();

        if (_content.HasContent())
        {
            return;
        }

        IReadOnlyDictionary<string, Guid> media = await _media.EnsureAsync();
        _content.Seed(media);
        _logger.LogInformation("Seeded the Rågklocka demo site ({MediaCount} images).", media.Count);
    }
}
