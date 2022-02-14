using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ropufu.Homepage.Data;

namespace Ropufu.Homepage.Pages.Xavier;

public class FacultyModel : PageModel
{
    private static readonly List<Person> s_no_people = new();
    private static readonly CacheOnceAgent<List<Person>> s_agent = new(FacultyModel.s_no_people);
    private static readonly MemoryCacheEntryOptions s_options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
    };

    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public FacultyModel(ApplicationDbContext context, IMemoryCache memoryCache)
    {
        this._context = context;
        this._memoryCache = memoryCache;
    }

    public IList<Person> People { get; private set; } = FacultyModel.s_no_people;

    public void OnGet()
    {
        if (this._context.People is null)
            throw new ApplicationException();

        this.People = this._memoryCache.GetOrCreateOnce(
            CacheKeys.Faculty,
            FacultyModel.s_agent,
            FacultyModel.s_options,
            () => this._context.People
                .Where(p => p.IsParticipatingFaculty)
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Include(p => p.TeachingHistory)
                .ThenInclude(h => h.Course)
                .ToList());
    }
}
