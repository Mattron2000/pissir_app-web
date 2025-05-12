namespace Shared.DTOs.User;

public record class UserRegisterDTO()
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}

public record class UserEntityDTO(string Email, string Name, string Surname, string Type);

public record class UserLoginDTO()
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public record class UserUpdateTypeDTO()
{
    public string Email { get; set; } = string.Empty;
}
