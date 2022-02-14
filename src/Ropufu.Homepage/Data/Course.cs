using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ropufu.Homepage.Data;

[Flags]
public enum CourseFlags : byte
{
    None = 0,
    [Display(Name = "Math Perspectives")]
    MathPerspectives      = 0b00001,
    [Display(Name = "Quantitative Reasoning")]
    QuantitativeReasoning = 0b00010,
    [Display(Name = "Writing")]
    Writing               = 0b00100,
    [Display(Name = "Oral")]
    Oral                  = 0b01000
}

public class Course
{
    public const char PrefixSeparator = '-';
    public const char SuffixSeparator = ' ';
    public const int PrefixFixedLength = 4;
    public const int SuffixFixedLength = 4;
    public const int MinNumber = 100;
    public const int MaxNumber = 999;

    [Key]
    [Display(Description = "Course Id in the database.")]
    public int Id { get; set; }

    [Required]
    [StringLength(127, ErrorMessage = "Name cannot be longer than 127 characters.")]
    [Display(Description = "Course name.")]
    public string Name { get; set; } = "??";

    [Required]
    [StringLength(Course.PrefixFixedLength, MinimumLength = Course.PrefixFixedLength, ErrorMessage = "Prefix must take exactly 4 characters.")]
    [Display(Description = "Program prefix.")]
    [Column(TypeName = "nchar(4)")]
    public string Prefix { get; set; } = "????";

    [StringLength(Course.SuffixFixedLength, MinimumLength = Course.SuffixFixedLength, ErrorMessage = "Suffix must take exactly 4 characters.")]
    [Display(Description = "Program suffix.")]
    [Column(TypeName = "nchar(4)")]
    public string? Suffix { get; set; }

    [Required]
    [Range(Course.MinNumber, Course.MaxNumber, ErrorMessage = "Number cannot be longer than 5 characters.")]
    [Display(Description = "Course number in the program.")]
    public int Number { get; set; } = Course.MinNumber;

    [Display(Description = "Number of hours per week the course meets.")]
    public byte CreditHours { get; set; }

    [Display(Description = "Number of hours per week the course contributes to the GPA.")]
    public byte ContactHours { get; set; }

    [Display(Description = "Flags that the course carries.")]
    public CourseFlags Flags { get; set; }

    [Display(Description = "Maximal number of flags granted by the course.")]
    public byte MaxFlagsGranted { get; set; }

    // Indicates if a student can be placed in the course based on results in some external test.
    public bool AllowsPlacement { get; set; }

    // Indicates if the instructor can waive the prerequisites.
    public bool AllowsInstructorConsent { get; set; }

    public bool RunsInEvenSpring { get; set; }

    public bool RunsInOddSpring { get; set; }

    public bool RunsInEvenFall { get; set; }

    public bool RunsInOddFall { get; set; }

    [StringLength(127, ErrorMessage = "Tags cannot be longer than 127 characters.")]
    [Display(Description = "Comma-separated tags.")]
    public string? Tags { get; set; }

    [Display(Description = "Course description.")]
    public string? Description { get; set; }
    
    [StringLength(255, ErrorMessage = "Comments cannot be longer than 127 characters.")]
    [Display(Description = "Misc. comments.")]
    public string? Comment { get; set; }

    [InverseProperty("Course")]
    [Display(Description = "Courses that have to be taken before or alongside this course.")]
    public ICollection<Prerequisite> Prerequisites { get; private set; } = new List<Prerequisite>();

    [InverseProperty("Course")]
    [Display(Description = "Teaching history for this course.")]
    public ICollection<TeachingJob> TeachingHistory { get; private set; } = new HashSet<TeachingJob>();

    //[NotMapped]
    //public string Affix => this.SuffixDisplay.Length == 0 ?
    //    this.PrefixDisplay :
    //    $"{this.PrefixDisplay} / {this.SuffixDisplay}";

    [NotMapped]
    public string CatalogueId => $"{this.Prefix.Trim()}{Course.PrefixSeparator}{this.Number}";

    [NotMapped]
    public string CatalogueIdWithSuffix => string.IsNullOrWhiteSpace(this.Suffix) ?
        this.CatalogueId :
        $"{this.CatalogueId}{Course.SuffixSeparator}{this.Suffix.Trim()}";
}
