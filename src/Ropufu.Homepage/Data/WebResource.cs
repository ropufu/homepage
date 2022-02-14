using System.ComponentModel.DataAnnotations;

namespace Ropufu.Homepage.Data;

public class WebResource
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(63, ErrorMessage = "Category cannot be longer than 63 characters.")]
    public string Category { get; set; } = "??";

    [Required]
    [StringLength(511, ErrorMessage = "UniformResourceLocator cannot be longer than 511 characters.")]
    public string UniformResourceLocator { get; set; } = "??";

    [Required]
    [StringLength(127, ErrorMessage = "Name cannot be longer than 127 characters.")]
    public string Name { get; set; } = "??";

    [Required]
    public string Description { get; set; } = "??";

    public string? Comment { get; set; }
}
