using Backend.Models;
using Backend.Repositories.Interfaces;
using Shared.DTOs.User;

namespace Backend.Services;

public enum UserResultEnum
{
    Success,
    Failed,
    UserAlreadyExists,
    UserNotFound,
    Forbid
}

public class UserResponse
{
    public UserResultEnum Result { get; init; }
    public UserEntityDTO? User { get; init; }
    public string? ErrorMessage { get; init; }

    public static UserResponse Success(UserEntityDTO user) =>
        new()
        {
            Result = UserResultEnum.Success,
            User = user
        };

    public static UserResponse Failed(UserResultEnum result = UserResultEnum.Failed, string? reason = null) =>
        new()
        {
            Result = result,
            ErrorMessage = reason ?? result switch
            {
                UserResultEnum.UserAlreadyExists => "User already exists",
                UserResultEnum.UserNotFound => "User not found",
                UserResultEnum.Forbid => "Forbidden",
                _ => null
            }
        };
}

public class UserService(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    internal async Task<UserResponse> CreateUserByRegistrationAsync(UserRegisterDTO userDto)
    {
        if (await _repository.GetUserByEmailAsync(userDto.Email) != null)
            return UserResponse.Failed(UserResultEnum.UserAlreadyExists);

        await _repository.InsertUserAsync(userDto.Email, userDto.Password, userDto.Name, userDto.Surname);

        return UserResponse.Success(
            new UserEntityDTO(
                userDto.Email,
                userDto.Name,
                userDto.Surname,
                UsersTypeEnum.BASE.ToString()
            )
        );
    }

    internal async Task<UserResponse> GetUserByLoginAsync(UserLoginDTO userDto)
    {
        User? user = await _repository.GetUserByEmailAsync(userDto.Email);

        if (user == null || user.Password != userDto.Password)
            return UserResponse.Failed(UserResultEnum.UserNotFound);

        return UserResponse.Success(
            new UserEntityDTO(
                user.Email,
                user.Name,
                user.Surname,
                user.Type
            )
        );
    }

    internal async Task<UserResponse> SwitchUserTypeAsync(string email)
    {
        User? user = await _repository.GetUserByEmailAsync(email);

        if (user == null)
            return UserResponse.Failed(UserResultEnum.UserNotFound);

        if (user.Type == UsersTypeEnum.ADMIN.ToString())
            return UserResponse.Failed(UserResultEnum.Forbid, "The admin cannot change type");

        UsersTypeEnum newType = Enum.Parse<UsersTypeEnum>(user.Type) switch
        {
            UsersTypeEnum.BASE => UsersTypeEnum.PREMIUM,
            UsersTypeEnum.PREMIUM => UsersTypeEnum.BASE,
            _ => throw new NotImplementedException()
        };

        await _repository.SetUserTypeAsync(email, newType.ToString());

        return UserResponse.Success(
            new UserEntityDTO(
                user.Email,
                user.Name,
                user.Surname,
                user.Type
            )
        );
    }

    internal async Task<UserResponse> GetUserByEmailAsync(string email)
    {
        User? user = await _repository.GetUserByEmailAsync(email);

        if (user == null) return UserResponse.Failed(UserResultEnum.UserNotFound);
        else return UserResponse.Success(
            new UserEntityDTO(
                user.Email,
                user.Name,
                user.Surname,
                user.Type
            )
        );
    }
}
