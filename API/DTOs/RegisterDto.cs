using System.ComponentModel.DataAnnotations;

namespace API;

public class RegisterDto
{
    [Required]
    [MaxLength(100)]
    public required string Username { set; get;}
    [Required]
    public required string Password { set; get; }
}
