using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class UserRegisterDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [RegularExpression(@"^[A-Z][a-zA-Z'\s]*$", ErrorMessage = "Name must contain only letters, start with a capital letter and may contain apostrophes for composite names")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    [RegularExpression(@"^[A-Z][a-zA-Z'\s]*$", ErrorMessage = "Name must contain only letters, start with a capital letter and may contain apostrophes for composite names")]
    public string Surname { get; set; }
}
