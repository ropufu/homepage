using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ropufu.Homepage.Data;

public class Person
{
    [Key]
    [Display(Description = "Person Id in the database.")]
    public int Id { get; set; }

    [Display(Name = "ASP.NET User Id")]
    public int? AspNetUserId { get; set; }

    [Required]
    [StringLength(127, ErrorMessage = "First name cannot be longer than 127 bytes.")]
    [Display(Name = "First Name", Description = "Your first name.")]
    public string FirstName { get; set; } = "??";

    [Required]
    [StringLength(127, ErrorMessage = "Last name cannot be longer than 127 bytes.")]
    [Display(Name = "Last Name", Description = "Your last name.")]
    public string LastName { get; set; } = "??";

    [Required]
    [StringLength(127, ErrorMessage = "Preferred name cannot be longer than 127 bytes.")]
    [Display(Name = "Preferred Name", Description = "How you would like to be addressed.")]
    public string PreferredName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Participating Faculty", Description = "Indicates if the person is participating faculty with the Math Department.")]
    public bool IsParticipatingFaculty { get; set; }

    [StringLength(511, ErrorMessage = "Homepage cannot be longer than 511 bytes.")]
    [Display(Name = "Homepage Url", Description = "Your homepage (if any).")]
    public string? Homepage { get; set; }

    [StringLength(63, ErrorMessage = "Email cannot be longer than 63 bytes.")]
    [Display(Name = "Email", Description = "Your email address (if any).")]
    public string? Email { get; set; }

    [InverseProperty("Person")]
    [Display(Description = "Teaching history for this person.")]
    public ICollection<TeachingJob> TeachingHistory { get; private set; } = new List<TeachingJob>();

    [NotMapped]
    public string FullName => $"{this.FirstName} {this.LastName}";

    [NotMapped]
    public string AugmentedLastName => this.FirstName.Length == 0 ?
        this.LastName :
        $"{this.FirstName[0]}. {this.LastName}";
}
