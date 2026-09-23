using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace HeadlessDemo.Cms.Seeding;

/// <summary>
/// Imports the photos in Seeding/Media into a media library folder and sets their alt text,
/// plus any cinema scroll layers found in Seeding/Media/Layers (transparent PNGs, optional).
/// Items that already exist (matched by name) are reused, so a second run imports nothing.
/// </summary>
public sealed class MediaSeeder
{
    private const string FolderName = "Site photos";

    // File name (without extension) -> alt text. Photo credits are listed in the README.
    private static readonly IReadOnlyDictionary<string, string> Photos = new Dictionary<string, string>
    {
        ["hero-flour"] = "A baker clapping flour off their hands over a white baking dish",
        ["loaf"] = "A round sourdough loaf on a wooden board next to a jar, a kettle and a mug",
        ["kneading"] = "Hands shaping soft dough on a floured wooden table",
        ["cafe-interior"] = "A bright café with a pastry counter, bentwood chairs and a long wooden table",
        ["rye-field"] = "A tractor track running through a ripe grain field at the edge of a forest",
        ["cardamom-bun"] = "A twisted cardamom bun on a white plate",
        ["cinnamon-bun-coffee"] = "A cinnamon bun on a small plate next to a cup of coffee",
        ["croissants"] = "Rows of freshly baked croissants on baking paper",
        ["coffee"] = "Two cups of coffee, one with latte art, on a dark wooden table",
    };

    // Cinema scroll layers, card icons and bake photos. Decorative, so no alt text. Missing files are skipped
    // and the content seeder falls back to a photo or leaves the layer empty.
    private static readonly string[] Layers =
    [
        "layer-sky", "layer-back", "layer-foreground", "layer-split-left", "layer-close-up",
        "icon-loaf", "icon-bun", "icon-cup",
        "bake-rye-loaf", "bake-wheat-loaf", "bake-cardamom-bun", "bake-cinnamon-bun", "bake-crispbread", "bake-croissant",
    ];

    private readonly IMediaService _mediaService;
    private readonly IMediaImportService _mediaImportService;
    private readonly IWebHostEnvironment _environment;

    public MediaSeeder(IMediaService mediaService, IMediaImportService mediaImportService, IWebHostEnvironment environment)
    {
        _mediaService = mediaService;
        _mediaImportService = mediaImportService;
        _environment = environment;
    }

    /// <summary>Returns media keys by photo name, e.g. "loaf".</summary>
    public async Task<IReadOnlyDictionary<string, Guid>> EnsureAsync()
    {
        IMedia folder = EnsureFolder();
        Dictionary<string, IMedia> existing = _mediaService
            .GetPagedChildren(folder.Id, 0, 100, out _)
            .ToDictionary(media => media.Name ?? string.Empty);

        var keys = new Dictionary<string, Guid>();
        string sourceDir = Path.Combine(_environment.ContentRootPath, "Seeding", "Media");

        foreach (var (name, altText) in Photos)
        {
            if (!existing.TryGetValue(name, out IMedia? media))
            {
                string path = Path.Combine(sourceDir, $"{name}.jpg");
                await using FileStream stream = System.IO.File.OpenRead(path);
                media = await _mediaImportService.ImportAsync(
                    $"{name}.jpg",
                    stream,
                    folder.Key,
                    Constants.Conventions.MediaTypes.Image,
                    Constants.Security.SuperUserKey);
                media.Name = name;
                media.SetValue("altText", altText);
                _mediaService.Save(media);
            }

            keys[name] = media.Key;
        }

        foreach (string name in Layers)
        {
            string path = Path.Combine(sourceDir, "Layers", $"{name}.png");
            if (!existing.TryGetValue(name, out IMedia? media))
            {
                if (!System.IO.File.Exists(path))
                {
                    continue;
                }

                await using FileStream stream = System.IO.File.OpenRead(path);
                media = await _mediaImportService.ImportAsync(
                    $"{name}.png",
                    stream,
                    folder.Key,
                    Constants.Conventions.MediaTypes.Image,
                    Constants.Security.SuperUserKey);
                media.Name = name;
                _mediaService.Save(media);
            }

            keys[name] = media.Key;
        }

        return keys;
    }

    private IMedia EnsureFolder()
    {
        IMedia? folder = _mediaService.GetRootMedia().FirstOrDefault(media => media.Name == FolderName);
        if (folder is not null)
        {
            return folder;
        }

        folder = _mediaService.CreateMedia(FolderName, Constants.System.Root, Constants.Conventions.MediaTypes.Folder);
        _mediaService.Save(folder);
        return folder;
    }
}
