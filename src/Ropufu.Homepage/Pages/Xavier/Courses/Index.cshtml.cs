using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Ropufu.Homepage.Data;

namespace Ropufu.Homepage.Pages.Xavier.Courses;

using NoCoursesGrouping = EmptyGrouping<int, Course>;
using ICourseGrouping = IGrouping<int, Course>;
using ICoursesByLevel = IEnumerable<IGrouping<int, Course>>;

public class IndexModel : PageModel
{
    private static readonly List<NoCoursesGrouping> s_no_courses = new();
    private static readonly CacheOnceAgent<ICoursesByLevel> s_agent = new(IndexModel.s_no_courses);
    private static readonly MemoryCacheEntryOptions s_options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
    };

    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public IndexModel(ApplicationDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public ICoursesByLevel CoursesByLevel { get; private set; } = IndexModel.s_no_courses;

    public string JavascriptGroupingKeys { get; private set; } = string.Empty;

    public string JavascriptItems { get; private set; } = string.Empty;

    public static string HtmlGroupingId(ICourseGrouping grouping) => $"ropufu-grouping-{grouping.Key}";

    public static string HtmlItemId(Course item) => $"ropufu-item-{item.Id}";

    public static IEnumerable<string> InitializeKeywords(Course item)
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
        keywords.Add(item.Number.ToString());
        keywords.Add(item.Prefix);

        return keywords;
    }

    private void InitializeJavascriptSearch()
    {
        JavascriptSearch.Initialize(this.CoursesByLevel,
            grouping => IndexModel.HtmlGroupingId(grouping),
            item => IndexModel.HtmlItemId(item),
            item => IndexModel.InitializeKeywords(item),
            out string keysCode, out string itemsCode);

        this.JavascriptGroupingKeys = keysCode;
        this.JavascriptItems = itemsCode;
    }

    public void OnGet()
    {
        if (_context.Courses is null)
            throw new ApplicationException();

        this.CoursesByLevel = _memoryCache.GetOrCreateOnce(
            CacheKeys.CoursesByLevel,
            IndexModel.s_agent,
            IndexModel.s_options,
            () => _context.Courses
                .OrderBy(l => l.Number)
                .ToList()
                .GroupBy(l => l.Number / 100));

        this.InitializeJavascriptSearch();
    }
}
