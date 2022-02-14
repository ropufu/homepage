using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ropufu.Homepage.Data;

public class Prerequisite
{
    [Required]
    [Display(Description = "Id of the course that has this prerequisite.")]
    public int CourseId { get; set; }

    [Required]
    [Display(Description = "Id of the prerequisite course.")]
    public int RequiredCourseId { get; set; }

    [Display(Description = "Minimum prerequisite course grade required.")]
    [Column(TypeName = "nchar(1)")]
    public Char RequiredCourseGrade { get; set; }

    [Display(Description = "Indicates if the course is recommended rather than required.")]
    public bool IsRecommended { get; set; }

    [Display(Description = "Indicates if the prerequisite can be taken at the same time as the course.")]
    public bool IsCorequisite { get; set; }

    [Display(Description = "Groups this prerequisite with alternative prerequisites for the same course.")]
    public byte AlternativesGroup { get; set; }

    [ForeignKey("CourseId")]
    public Course? Course { get; set; }

    [ForeignKey("RequiredCourseId")]
    public Course? RequiredCourse { get; set; }
}
