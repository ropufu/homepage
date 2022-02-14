using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ropufu.Homepage.Data;

public enum SchoolSemester : byte
{
    [Display(Name = "Spring Semester")]
    Spring = 0,
    [Display(Name = "Summer Semester")]
    Summer = 1,
    [Display(Name = "Fall Semester")]
    Fall = 2
}

public readonly record struct SchoolTerm(int Year, SchoolSemester Semester) : IComparable<SchoolTerm>
{
    public int CompareTo(SchoolTerm other) => this.GetHashCode() - other.GetHashCode();

    public override int GetHashCode() => (this.Year << 4) | (int)this.Semester;

    public static bool operator <(SchoolTerm left, SchoolTerm right) => left.CompareTo(right) < 0;
    public static bool operator <=(SchoolTerm left, SchoolTerm right) => left.CompareTo(right) <= 0;
    public static bool operator >(SchoolTerm left, SchoolTerm right) => left.CompareTo(right) > 0;
    public static bool operator >=(SchoolTerm left, SchoolTerm right) => left.CompareTo(right) >= 0;
}

public class TeachingJob
{
    public const int SectionFixedLength = 3;
    public const int CRNFixedLength = 5;

    [Key]
    [Display(Description = "Teaching Job Id in the database.")]
    public int Id { get; set; }

    // Course Id in the database.
    [Required]
    public int CourseId { get; set; }

    // Instructor Id in the database.
    [Required]
    public int PersonId { get; set; }

    [Required]
    [Display(Description = "Year the class was taught.")]
    public int Year { get; set; }

    [Required]
    [Display(Description = "Semester the class was taught.")]
    public SchoolSemester Semester { get; set; }

    [Required]
    [StringLength(TeachingJob.SectionFixedLength, ErrorMessage = "Section cannot be longer than 3 characters.")]
    [Display(Description = "Section number.")]
    public string Section { get; set; } = "??";

    [Required]
    [StringLength(TeachingJob.CRNFixedLength, ErrorMessage = "CRN cannot be longer than 5 characters.")]
    [Display(Description = "CRN.")]
    public string CourseReferenceNumber { get; set; } = "??";

    [Required]
    [Display(Description = "Class capacity.")]
    public byte StudentsCapacity { get; set; }

    [Required]
    [Display(Description = "Number of students enrolled in the class.")]
    public byte StudentsEnrolled { get; set; }

    [ForeignKey("CourseId")]
    public Course? Course { get; set; }

    [ForeignKey("PersonId")]
    public Person? Person { get; set; }

    [NotMapped]
    public SchoolTerm Term => new() { Year = this.Year, Semester = this.Semester };

    [NotMapped]
    public string SectionDisplay => this.Section.TrimEnd();

    [NotMapped]
    public string CourseReferenceNumberDisplay => this.CourseReferenceNumber.TrimEnd();
}
