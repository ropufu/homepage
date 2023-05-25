using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Ropufu.Homepage.Data;

namespace Ropufu.Homepage.Pages;

using NoLinksGrouping = EmptyGrouping<string, WebResource>;
using ILinkGrouping = IGrouping<string, WebResource>;
using ILinksByCategory = IEnumerable<IGrouping<string, WebResource>>;

public class LinksModel : PageModel
{
    private static readonly List<NoLinksGrouping> s_no_links = new();
    private static readonly CacheOnceAgent<ILinksByCategory> s_agent = new(LinksModel.s_no_links);
    private static readonly MemoryCacheEntryOptions s_options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
    };

    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public LinksModel(ApplicationDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public ILinksByCategory LinksByCategory { get; private set; } = LinksModel.s_no_links;

    public string JavascriptGroupingKeys { get; private set; } = string.Empty;

    public string JavascriptItems { get; private set; } = string.Empty;

    public static string HtmlGroupingId(ILinkGrouping grouping) => $"ropufu-grouping-{grouping.Key.GetHashCode()}";

    public static string HtmlItemId(WebResource item) => $"ropufu-item-{item.Id}";

    public static IEnumerable<string> InitializeKeywords(WebResource item)
    {
        List<string> keywords = new();
        foreach (string word in item.Name.Split(' '))
        {
            string safe = word.Reduce();
            if (safe.Length == 0)
                continue;
            if (!safe.IsTooShort())
                keywords.Add(safe.ToLowerInvariant());
        } // foreach (...)
        foreach (string word in item.Description.Split(' '))
        {
            string safe = word.Reduce();
            if (safe.Length == 0)
                continue;
            if (!safe.IsTooShort())
                keywords.Add(safe.ToLowerInvariant());
        } // foreach (...)

        return keywords;
    }

    private void InitializeJavascriptSearch()
    {
        JavascriptSearch.Initialize(this.LinksByCategory,
            grouping => LinksModel.HtmlGroupingId(grouping),
            item => LinksModel.HtmlItemId(item),
            item => LinksModel.InitializeKeywords(item),
            out string keysCode, out string itemsCode);

        this.JavascriptGroupingKeys = keysCode;
        this.JavascriptItems = itemsCode;
    }

    public void OnGet()
    {
        if (_context.WebResources is null)
            throw new ApplicationException();

        this.LinksByCategory = _memoryCache.GetOrCreateOnce(
            CacheKeys.LinksByCategory,
            LinksModel.s_agent,
            LinksModel.s_options,
            () => _context.WebResources
                .OrderBy(l => l.Category)
                .ThenBy(l => l.Name)
                .ToList()
                .GroupBy(l => l.Category));

        this.InitializeJavascriptSearch();
    }
}
