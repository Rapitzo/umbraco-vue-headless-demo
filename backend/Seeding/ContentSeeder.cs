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

        SetHome(home, media, bread, news, visit);
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

    private void SetHome(IContent home, IReadOnlyDictionary<string, Guid> media, IContent bread, IContent news, IContent visit)
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
            Hero(
                "Rye bread and slow mornings.",
                "We bake sourdough rye, wheat loaves and cardamom buns every morning in the room behind the counter. Come early for the first bake, or late for whatever is left.",
                media["hero-flour"],
                "See what we bake",
                bread),
            BakeBoard(
                "Dagens bröd",
                "Today's bake, with the time each batch came out of the oven. Prices in SEK.",
                Bake("Rye loaf, sunflower seeds", "Rågbröd · 900 g", "07:10", 68),
                Bake("Country wheat loaf", "Lantbröd · 750 g", "07:40", 62),
                Bake("Cardamom bun", "Kardemummabulle", "08:15", 38),
                Bake("Cinnamon bun", "Kanelbulle", "08:15", 36),
                Bake("Rye crispbread", "Knäckebröd · 250 g bag", "06:40", 55),
                Bake("Croissant", "Saturdays only", "09:00", 34, soldOut: true)),
            Story(
                "A bakery with a café in the front",
                "<p>The ovens are twelve steps from the tables. Most mornings you can hear the first loaves crackle as they cool on the rack by the window.</p>" +
                "<p>We serve coffee from a small roaster, buns straight from the tray and open sandwiches on our own rye until the bread runs out.</p>",
                media["cafe-interior"],
                "The café seats twenty-two. The long table is first come, first served."),
            CardRow(
                "Plan your visit",
                Card("Our bread", "Rye, wheat and what we bake on which day.", media["loaf"], bread),
                Card("News from the bakery", "Seasonal bakes, harvest notes and changes to opening hours.", media["croissants"], news),
                Card("Find us", "Opening hours, the address and how to order for a group.", media["coffee"], visit)),
            Quote("Rye takes its time. We let it.", "Written above the proofing shelf"));
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

    private static GridBlock Hero(string heading, string text, Guid image, string ctaLabel, IContent ctaLink) =>
        new("heroBlock", 12, new Dictionary<string, object?>
        {
            ["heading"] = heading,
            ["text"] = text,
            ["image"] = BlockGridValue.Image(image),
            ["ctaLabel"] = ctaLabel,
            ["ctaLink"] = BlockGridValue.Document(ctaLink.Key),
        });

    private GridBlock BakeBoard(string heading, string intro, params GridBlock[] bakes) =>
        new("bakeBoardBlock", 12, new Dictionary<string, object?>
        {
            ["heading"] = heading,
            ["intro"] = intro,
            ["items"] = BlockGridValue.BlockList(bakes, ElementTypeKey),
        });

    private static GridBlock Bake(string name, string note, string readyAt, int price, bool soldOut = false) =>
        new("bakeItemBlock", 12, new Dictionary<string, object?>
        {
            ["name"] = name,
            ["note"] = note,
            ["readyAt"] = readyAt,
            ["price"] = price,
            ["soldOut"] = soldOut,
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

    private static GridBlock CardRow(string heading, params GridBlock[] cards) =>
        new("cardRowBlock", 12, new Dictionary<string, object?> { ["heading"] = heading })
        {
            Area = new GridArea(SchemaSeeder.CardsAreaKey, cards),
        };

    private static GridBlock Card(string title, string text, Guid image, IContent link) =>
        new("cardBlock", 4, new Dictionary<string, object?>
        {
            ["title"] = title,
            ["text"] = text,
            ["image"] = BlockGridValue.Image(image),
            ["link"] = BlockGridValue.Document(link.Key),
        });
}
