using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace HeadlessDemo.Cms.Seeding;

public sealed class SeedingComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTransient<SchemaSeeder>();
        builder.Services.AddTransient<MediaSeeder>();
        builder.Services.AddTransient<ContentSeeder>();
        builder.AddNotificationAsyncHandler<UmbracoApplicationStartedNotification, SiteSeeder>();
    }
}
