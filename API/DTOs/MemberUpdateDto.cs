using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class MemberUpdateDto
{
    [Required]
    public string Introduction { get; set; } = null!;

    [Required]
    public string LookingFor { get; set; } = null!;

    [Required]
    public string Interests { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    [Required]
    public string Country { get; set; } = null!;
}
