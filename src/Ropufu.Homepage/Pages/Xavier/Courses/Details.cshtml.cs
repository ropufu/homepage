using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ropufu.Homepage.Data;

namespace Ropufu.Homepage.Pages.Xavier.Courses;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Course? SingleCourse { get; private set; }

    public string CourseTitle => this.SingleCourse?.Name ?? "Course not found.";

    public void OnGet(string prefix, int number)
    {
        if (_context.Courses is null)
            throw new ApplicationException();

        prefix = prefix.Reduce().ToUpperInvariant();

        this.SingleCourse = _context.Courses
            .Include(c => c.Prerequisites)
            .ThenInclude(p => p.RequiredCourse)
            .Include(c => c.TeachingHistory)
            .ThenInclude(h => h.Person)
            .SingleOrDefault(c => c.Number == number && c.Prefix == prefix);
    }
}
