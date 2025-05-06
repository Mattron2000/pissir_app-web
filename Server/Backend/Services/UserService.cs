using Backend.Models;
using Backend.Repositories;
using Shared.DTOs;

namespace Backend.Services;

public enum UserResultEnum
{
    Success,
    Failed,
    UserAlreadyExists,
    UserInsertFailed,
    BadRequest
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
                UserResultEnum.UserInsertFailed => "An error occurred while attempting to register the user",
                _ => null
            }
        };
}

public class UserService(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    internal async Task<UserResponse> CreateUserByRegistrationAsync(UserRegisterDTO userDto)
    {
        if (await _repository.CheckUserIfExistsByEmailAsync(userDto.Email))
            return UserResponse.Failed(UserResultEnum.UserAlreadyExists);

        if (!await _repository.InsertUserAsync(userDto.Email, userDto.Password, userDto.Name, userDto.Surname))
            return UserResponse.Failed(UserResultEnum.UserInsertFailed);

        return UserResponse.Success(
            new UserEntityDTO(
                userDto.Email,
                userDto.Name,
                userDto.Surname,
                UsersTypeEnum.BASE.ToString()
            )
        );
    }
}
