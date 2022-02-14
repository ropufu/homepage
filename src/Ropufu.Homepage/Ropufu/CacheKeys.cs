namespace Ropufu.Homepage;

public static class CacheKeys
{
    public static readonly string LinksByCategory =
        nameof(Pages) +
        nameof(Pages.LinksModel) +
        nameof(Pages.LinksModel.LinksByCategory);

    public static readonly string CoursesByLevel =
        nameof(Pages) +
        nameof(Pages.Xavier) +
        nameof(Pages.Xavier.Courses) +
        nameof(Pages.Xavier.Courses.IndexModel) +
        nameof(Pages.Xavier.Courses.IndexModel.CoursesByLevel);

    public static readonly string Faculty =
        nameof(Pages) +
        nameof(Pages.Xavier) +
        nameof(Pages.Xavier.FacultyModel) +
        nameof(Pages.Xavier.FacultyModel.People);
}
