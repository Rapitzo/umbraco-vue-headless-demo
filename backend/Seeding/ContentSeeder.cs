using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace HeadlessDemo.Cms.Seeding;

/// <summary>
/// Builds and publishes the site for the fictional bakery Rågklocka. Skips if any root content
/// already exists, so editors' changes are never overwritten.
/// </summary>
public sealed class ContentSeeder
{
    private readonly IContentService _contentService;
    private readonly IContentTypeService _contentTypeService;

    public ContentSeeder(IContentService contentService, IContentTypeService contentTypeService)
    {
        _contentService = contentService;
        _contentTypeService = contentTypeService;
    }

    public bool HasContent() => _contentService.GetRootContent().Any();

    public void Seed(IReadOnlyDictionary<string, Guid> media)
    {
        IContent home = Save(_contentService.Create("Rågklocka", Constants.System.Root, "home"));
        IContent bread = Save(_contentService.Create("Our bread", home, "contentPage"));
        IContent about = Save(_contentService.Create("About us", home, "contentPage"));
        IContent news = Save(_contentService.Create("News", home, "newsList"));
        IContent visit = Save(_contentService.Create("Visit us", home, "contactPage"));

        SetHome(home, media, bread, about, news, visit);
        SetBread(bread, media);
        SetAbout(about, media);
        SetNewsList(news);
        SetVisit(visit, media);
        foreach (IContent content in new[] { home, bread, about, news, visit })
        {
            _contentService.Save(content);
        }

        SeedNewsItems(news, media);

        _contentService.PublishBranch(home, PublishBranchFilter.IncludeUnpublished, ["*"]);
    }

    private IContent Save(IContent content)
    {
        _contentService.Save(content);
        return content;
    }

    private void SetBlocks(IContent content, params GridBlock[] blocks) =>
        content.SetValue("blocks", BlockGridValue.Serialize(blocks, ElementTypeKey));

    private Guid ElementTypeKey(string alias) =>
        _contentTypeService.Get(alias)?.Key ?? throw new InvalidOperationException($"Element type '{alias}' not found.");

    private void SetHome(IContent home, IReadOnlyDictionary<string, Guid> media, IContent bread, IContent about, IContent news, IContent visit)
    {
        home.SetValue("siteName", "Rågklocka");
        home.SetValue("logoText", "Rågklocka");
        home.SetValue("tagline", "Sourdough bakery & café");
        home.SetValue("address", "Bagargränd 7\n123 45 Exempelstad");
        home.SetValue("openingHours", "Monday to Friday, 07:00 to 18:00\nSaturday, 08:00 to 16:00\nSunday, closed");
        home.SetValue("phone", "070-174 06 42");
        home.SetValue("email", "hej@ragklocka.example");
        home.SetValue("footerNote", "Rågklocka is a fictional bakery made up for a demo site. The address, phone number and email are not real.");
        home.SetValue("metaDescription", "Rågklocka is a sourdough bakery and café. Rye bread, cardamom buns and coffee, baked on site from seven every morning.");

        SetBlocks(
            home,
            Cinema(
                media,
                visit,
                new Scene("Rågklocka", Tags: "Sourdough rye, Cardamom buns, Café"),
                new Scene(
                    "The rye sets the clock.",
                    "The first loaves come out at ten past seven. The dough has been rising since the afternoon before.",
                    Facts: ("36 h", "Longest rise for the rye", "07:10", "First loaf out of the oven")),
                new Scene(
                    "The café is twelve steps away.",
                    "Coffee from a small roaster, buns straight from the tray and open sandwiches on our own rye.",
                    Cta: "Plan your visit"),
                new Board(
                    "Dagens bröd",
                    "Today's bake, with the time each batch came out of the oven. Prices in SEK.",
                    [
                        Bake("Seeded rye loaf", "Rågbröd · 900 g", "07:10", 68, OptionalImage(media, "bake-rye-loaf")),
                        Bake("Country wheat loaf", "Lantbröd · 750 g", "07:40", 62, OptionalImage(media, "bake-wheat-loaf")),
                        Bake("Cardamom bun", "Kardemummabulle", "08:15", 38, OptionalImage(media, "bake-cardamom-bun")),
                        Bake("Cinnamon bun", "Kanelbulle", "08:15", 36, OptionalImage(media, "bake-cinnamon-bun")),
                        Bake("Rye crispbread", "Knäckebröd · 250 g bag", "06:40", 55, OptionalImage(media, "bake-crispbread")),
                        Bake("Croissant", "Saturdays only", "09:00", 34, OptionalImage(media, "bake-croissant"), soldOut: true),
                    ]),
                CinemaCard("From 07:00", "First bake", "Rye and country loaves, still crackling on the rack by the window.", media, "icon-loaf", bread),
                CinemaCard("Tue to Sat", "Cardamom buns", "Butter, fresh cardamom and a long cold rise. On Fridays, all day.", media, "icon-bun", bread),
                CinemaCard("Saturday", "Croissants", "One batch of ninety, three days in the making. Gone by eleven.", media, "icon-bun", news),
                CinemaCard("All day", "Open sandwiches", "On our own rye, served at the long table until the bread runs out.", media, "icon-cup", visit),
                CinemaCard("Harvest", "Local grain", "Rye grown twenty minutes north and stone-milled in small batches.", media, "icon-loaf", about)));
    }

    private void SetBread(IContent page, IReadOnlyDictionary<string, Guid> media)
    {
        page.SetValue("intro", "Everything on the counter is baked here, from flour we can trace back to the mill.");
        page.SetValue("metaDescription", "Sourdough rye, country wheat loaves, cardamom buns and weekend croissants. Here is what Rågklocka bakes and when.");
        SetBlocks(
            page,
            RichText(
                8,
                "<h2>Sourdough first</h2>" +
                "<p>Our rye starter is fed twice a day. The dough rests for up to 36 hours before it goes in the oven, which gives the bread its deep, slightly sour taste and a crumb that keeps for days.</p>" +
                "<p>We use no commercial yeast in the loaves. The buns and croissants get a little, because butter and patience only go so far.</p>"),
            Image(12, media["loaf"], "The house rye: dark crust, dense crumb, sunflower seeds."),
            RichText(
                6,
                "<h2>The daily bake</h2>" +
                "<ul>" +
                "<li><strong>Rye loaf with sunflower seeds</strong>, every day</li>" +
                "<li><strong>Country wheat loaf</strong>, every day</li>" +
                "<li><strong>Cardamom buns</strong>, Tuesday to Saturday, and Fridays all day</li>" +
                "<li><strong>Croissants</strong>, Saturday until sold out</li>" +
                "</ul>" +
                "<p>Want a loaf set aside? Call before ten and we will put your name on it.</p>"),
            Image(6, media["kneading"], "Every loaf is shaped by hand."),
            Quote("If the dough is not ready, the bread is not ready.", "The only rule in the bake room"));
    }

    private void SetAbout(IContent page, IReadOnlyDictionary<string, Guid> media)
    {
        page.SetValue("intro", "A small bakery that bakes a few things well and sells them warm.");
        page.SetValue("metaDescription", "How Rågklocka started, where the grain comes from and why the bakery opens at seven.");
        SetBlocks(
            page,
            Story(
                "How it started",
                "<p>Rågklocka opened in an old corner shop with one deck oven and a hand-written menu. The name means rye clock: the rye decides when the day starts.</p>" +
                "<p>Today there are two ovens, a café counter and a bake room that runs from four in the morning. The menu is still short on purpose.</p>",
                media["kneading"],
                "Every loaf is still shaped by hand.",
                imageOnLeft: true),
            Image(12, media["rye-field"], "Our rye and wheat are grown about twenty minutes north of the bakery."),
            RichText(
                8,
                "<h2>Where the grain comes from</h2>" +
                "<p>We buy whole grain from two farms in the region and have it stone-milled in small batches. Fresh flour behaves differently from week to week, so the recipes change a little with it.</p>" +
                "<p>Leftover bread goes to a local food bank at closing time, or into the next day's crispbread.</p>"));
    }

    private void SetNewsList(IContent page)
    {
        page.SetValue("intro", "Seasonal bakes, opening hours and the occasional note from the bake room.");
        page.SetValue("metaDescription", "News from Rågklocka: seasonal bakes, harvest notes and opening hours.");
    }

    private void SetVisit(IContent page, IReadOnlyDictionary<string, Guid> media)
    {
        page.SetValue("intro", "Walk in any day except Sunday. For bigger orders, call ahead or send us a note.");
        page.SetValue("image", BlockGridValue.ImageJson(media["cinnamon-bun-coffee"]));
        page.SetValue("formIntro", "Ordering for a meeting or a party? Tell us what you need and when, and we will get back to you within a working day.");
        page.SetValue("metaDescription", "Address, opening hours and contact details for Rågklocka sourdough bakery and café.");
    }

    // Created oldest first, the way an editor would add them. The frontend sorts by publish date.
    private void SeedNewsItems(IContent newsList, IReadOnlyDictionary<string, Guid> media)
    {
        var items = new[]
        {
            (
                Name: "Why the croissants are gone by eleven",
                Date: new DateTime(2026, 7, 30),
                Image: "croissants",
                Teaser: "We bake one batch of croissants on Saturdays, and it takes three days. Here is why we do not simply make more.",
                Blocks: new[]
                {
                    RichText(8, "<p>A croissant dough is folded, chilled and folded again over three days. We have room in the fridge for one batch, and one batch is about ninety croissants.</p><p>We could buy in frozen dough and bake all day. We would rather sell out early and bake them properly.</p>"),
                    Quote("Ninety croissants, three days, one fridge.", "The Saturday maths"),
                }),
            (
                Name: "Longer Saturday opening from September",
                Date: new DateTime(2026, 8, 14),
                Image: "coffee",
                Teaser: "From the first Saturday in September we open at eight and stay open until four.",
                Blocks: new[]
                {
                    RichText(8, "<p>Saturday mornings have been our busiest by far, so from September we open an hour earlier and close an hour later. Weekdays stay the same.</p><p>The café kitchen keeps serving open sandwiches until two.</p>"),
                }),
            (
                Name: "This year's rye harvest is in",
                Date: new DateTime(2026, 8, 27),
                Image: "rye-field",
                Teaser: "The new rye has been milled. Expect a slightly lighter loaf for the first few weeks while we get to know it.",
                Blocks: new[]
                {
                    RichText(8, "<p>Our rye comes from a farm twenty minutes north of the bakery. This year's grain is drier than last year's, so the dough takes more water.</p><p>We are adjusting the recipe a little each week. If your loaf tastes different in September, that is why.</p>"),
                    Image(12, media["kneading"], "Getting to know the new flour."),
                }),
            (
                Name: "Cardamom buns every Friday",
                Date: new DateTime(2026, 9, 11),
                Image: "cardamom-bun",
                Teaser: "Cardamom buns are back on the counter all day on Fridays, not just until the morning batch runs out.",
                Blocks: new[]
                {
                    RichText(8, "<p>We now bake a second batch of cardamom buns on Friday afternoons. The dough is the same: butter, freshly ground cardamom and a long cold rise overnight.</p><p>Want a tray for the office? Order by Thursday noon.</p>"),
                }),
        };

        foreach (var item in items)
        {
            IContent news = _contentService.Create(item.Name, newsList, "newsItem");
            news.SetValue("publishDate", item.Date);
            news.SetValue("teaser", item.Teaser);
            news.SetValue("image", BlockGridValue.ImageJson(media[item.Image]));
            SetBlocks(news, item.Blocks);
            _contentService.Save(news);
        }
    }

    private sealed record Scene(
        string Heading,
        string? Text = null,
        string? Tags = null,
        (string Value1, string Label1, string Value2, string Label2)? Facts = null,
        string? Cta = null);

    // Layers come from Seeding/Media/Layers when those files exist. Without them the sky and
    // close-up fall back to photos, and the cutout layers stay empty.
    private sealed record Board(string Heading, string Intro, GridBlock[] Bakes);

    private GridBlock Cinema(IReadOnlyDictionary<string, Guid> media, IContent ctaLink, Scene intro, Scene first, Scene second, Board board, params GridBlock[] cards)
    {
        object? Layer(string name, string? fallback = null) =>
            media.TryGetValue(name, out Guid key) || (fallback is not null && media.TryGetValue(fallback, out key))
                ? BlockGridValue.Image(key)
                : null;

        return new("cinemaBlock", 12, new Dictionary<string, object?>
        {
            ["heading"] = intro.Heading,
            ["intro"] = intro.Text,
            ["tags"] = intro.Tags,
            ["firstHeading"] = first.Heading,
            ["firstText"] = first.Text,
            ["fact1Value"] = first.Facts?.Value1,
            ["fact1Label"] = first.Facts?.Label1,
            ["fact2Value"] = first.Facts?.Value2,
            ["fact2Label"] = first.Facts?.Label2,
            ["secondHeading"] = second.Heading,
            ["secondText"] = second.Text,
            ["ctaLabel"] = second.Cta,
            ["ctaLink"] = BlockGridValue.Document(ctaLink.Key),
            ["cards"] = BlockGridValue.BlockList(cards, ElementTypeKey),
            ["boardHeading"] = board.Heading,
            ["boardIntro"] = board.Intro,
            ["bakes"] = BlockGridValue.BlockList(board.Bakes, ElementTypeKey),
            ["skyLayer"] = Layer("layer-sky", "rye-field"),
            ["backLayer"] = Layer("layer-back"),
            ["foregroundLayer"] = Layer("layer-foreground"),
            ["splitLeftLayer"] = Layer("layer-split-left"),
            ["closeUpLayer"] = Layer("layer-close-up", "loaf"),
        });
    }

    private static GridBlock CinemaCard(string kicker, string title, string text, IReadOnlyDictionary<string, Guid> media, string icon, IContent link) =>
        new("cinemaCardBlock", 12, new Dictionary<string, object?>
        {
            ["kicker"] = kicker,
            ["title"] = title,
            ["text"] = text,
            ["icon"] = OptionalImage(media, icon),
            ["link"] = BlockGridValue.Document(link.Key),
        });

    /// <summary>Media Picker value for an optional seeded file, or null when it was not imported.</summary>
    private static object? OptionalImage(IReadOnlyDictionary<string, Guid> media, string name) =>
        media.TryGetValue(name, out Guid key) ? BlockGridValue.Image(key) : null;

    private static GridBlock Bake(string name, string note, string readyAt, int price, object? image = null, bool soldOut = false) =>
        new("bakeItemBlock", 12, new Dictionary<string, object?>
        {
            ["name"] = name,
            ["note"] = note,
            ["readyAt"] = readyAt,
            ["price"] = price,
            ["soldOut"] = soldOut,
            ["image"] = image,
        });

    private static GridBlock Story(string heading, string markup, Guid image, string caption, bool imageOnLeft = false) =>
        new("storyBlock", 12, new Dictionary<string, object?>
        {
            ["heading"] = heading,
            ["text"] = BlockGridValue.RichText(markup),
            ["image"] = BlockGridValue.Image(image),
            ["caption"] = caption,
            ["imageOnLeft"] = imageOnLeft,
        });

    private static GridBlock RichText(int columnSpan, string markup) =>
        new("richTextBlock", columnSpan, new Dictionary<string, object?> { ["text"] = BlockGridValue.RichText(markup) });

    private static GridBlock Image(int columnSpan, Guid image, string caption) =>
        new("imageBlock", columnSpan, new Dictionary<string, object?> { ["image"] = BlockGridValue.Image(image), ["caption"] = caption });

    private static GridBlock Quote(string quote, string attribution) =>
        new("quoteBlock", 12, new Dictionary<string, object?> { ["quote"] = quote, ["attribution"] = attribution });
}
