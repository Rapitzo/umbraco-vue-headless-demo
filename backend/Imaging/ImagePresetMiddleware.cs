using System.Globalization;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Models;

namespace HeadlessDemo.Cms.Imaging;

/// <summary>
/// Lets a headless frontend request image renditions without knowing the imaging HMAC key.
/// A request such as <c>/media/abc/loaf.jpg?preset=standard&amp;width=800</c> is rewritten to a
/// signed ImageSharp query before the imaging middleware runs. Only the presets and widths
/// listed here can be produced, so clients cannot ask the server for arbitrary resizes.
/// </summary>
public sealed class ImagePresetMiddleware
{
    // Aspect ratio (width / height) per preset. "original" keeps the source ratio.
    private static readonly IReadOnlyDictionary<string, decimal?> Presets = new Dictionary<string, decimal?>
    {
        ["wide"] = 16m / 9m,
        ["standard"] = 4m / 3m,
        ["square"] = 1m,
        ["original"] = null,
    };

    private static readonly int[] Widths = [480, 800, 1200, 1600];

    private readonly RequestDelegate _next;

    public ImagePresetMiddleware(RequestDelegate next) => _next = next;

    public Task InvokeAsync(HttpContext context, IImageUrlGenerator imageUrlGenerator)
    {
        HttpRequest request = context.Request;
        if (request.Path.StartsWithSegments("/media")
            && Presets.TryGetValue(request.Query["preset"].ToString(), out decimal? ratio)
            && int.TryParse(request.Query["width"], NumberStyles.None, CultureInfo.InvariantCulture, out int width)
            && Widths.Contains(width))
        {
            var options = new ImageUrlGenerationOptions(request.Path.Value)
            {
                Width = width,
                Height = ratio is { } r ? (int)Math.Round(width / r) : null,
                ImageCropMode = ratio is null ? ImageCropMode.Max : ImageCropMode.Crop,
                FocalPoint = ReadFocalPoint(request.Query),
                Format = "webp",
                Quality = 75,
            };

            string? signedUrl = imageUrlGenerator.GetImageUrl(options);
            int queryStart = signedUrl?.IndexOf('?') ?? -1;
            if (queryStart >= 0)
            {
                request.QueryString = new QueryString(signedUrl![queryStart..]);
            }
        }

        return _next(context);
    }

    // Focal point from the Delivery API media item, as fractions of width and height (0 to 1).
    private static ImageUrlGenerationOptions.FocalPointPosition? ReadFocalPoint(IQueryCollection query) =>
        decimal.TryParse(query["fx"], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal left)
        && decimal.TryParse(query["fy"], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal top)
        && left is >= 0 and <= 1
        && top is >= 0 and <= 1
            ? new ImageUrlGenerationOptions.FocalPointPosition(left, top)
            : null;
}
