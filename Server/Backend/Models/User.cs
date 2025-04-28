namespace Backend.Models;

public class User
{
    public string Email { get; set; }
    public string Password { get; set; }
    public UserTypeEnum Type { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    // Navigation property
    public UserType UserType { get; set; }
}
