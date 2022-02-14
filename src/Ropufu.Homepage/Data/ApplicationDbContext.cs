using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ropufu.Homepage.Data;

public class ApplicationDbContext
    : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Make a composite primary key.
        builder.Entity<Prerequisite>()
            .HasKey(x => new { x.CourseId, x.RequiredCourseId });

        // @todo Consider adding a trigger to manually delete Prerequisites before a Course is deleted.
        builder.Entity<Prerequisite>()
            .HasOne<Course>(nameof(Prerequisite.Course))
                .WithMany(nameof(Course.Prerequisites))
                .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Prerequisite>()
            .HasOne<Course>(nameof(Prerequisite.RequiredCourse))
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

        // Make sure course {prefix}-{number} identification is unique.
        builder.Entity<Course>()
            .HasIndex(x => new { x.Prefix, x.Number });
    }

    public DbSet<TeachingJob>? TeachingHistory { get; set; }

    public DbSet<Person>? People { get; set; }

    public DbSet<Course>? Courses { get; set; }

    public DbSet<Prerequisite>? Prerequisites { get; set; }

    public DbSet<WebResource>? WebResources { get; set; }
}
