using Backend.Models;
using Backend.Repositories;
using FluentValidation;
using Shared.DTOs;

namespace Backend.Services;

public enum UserResultEnum
{
    Success,
    Failed,
    UserAlreadyExists,
    UserNotFound
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
                _ => null
            }
        };
}

public class UserService(IUserRepository repository, IValidator<UserRegisterDTO> validator)
{
    private readonly IValidator<UserRegisterDTO> _validator = validator;

    private readonly IUserRepository _repository = repository;

    internal async Task<UserResponse> CreateUserByRegistrationAsync(UserRegisterDTO userDto)
    {
        if (await _repository.CheckUserIfExistsByEmailAsync(userDto.Email))
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
        User? user = await _repository.GetUserByEmailAndPasswordAsync(userDto.Email, userDto.Password);

        if (user == null)
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
}
