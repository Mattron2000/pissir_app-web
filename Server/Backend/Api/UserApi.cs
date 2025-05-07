using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;

namespace Backend.Api;

internal interface IUserApi
{
    Task<Results<Created<UserEntityDTO>, Conflict<UserMessageDTO>, BadRequest<UserMessagesDTO>, ProblemHttpResult>> Register(UserRegisterDTO user, UserService service, IValidator<UserRegisterDTO> validator);
    Task<Results<Ok<UserEntityDTO>, NotFound<UserMessageDTO>, BadRequest<UserMessagesDTO>, ProblemHttpResult>> Login(UserLoginDTO userDto, UserService service, IValidator<UserLoginDTO> validator);
}

public class UserApi : IApiEndpoint, IUserApi
{
    private readonly string RootApiV1 = "/api/v1";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup(RootApiV1));
    }

    private void MapV1(RouteGroupBuilder userApi)
    {
        userApi.MapPost("/register", Register);
        userApi.MapPost("/login", Login);
    }

    public async Task<Results<Created<UserEntityDTO>, Conflict<UserMessageDTO>, BadRequest<UserMessagesDTO>, ProblemHttpResult>> Register(UserRegisterDTO userDto, UserService service, IValidator<UserRegisterDTO> validator)
    {
        var result = await validator.ValidateAsync(userDto);
        if (!result.IsValid)
            return TypedResults.BadRequest(new UserMessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        UserResponse response = await service.CreateUserByRegistrationAsync(userDto);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Registration Failed: response is null"
            );

        if (response.Result == UserResultEnum.Success && response.User != null)
            return TypedResults.Created($"/api/v1/users/{response.User.Email}", response.User);

        if (response.Result == UserResultEnum.UserAlreadyExists && response.ErrorMessage != null)
            return TypedResults.Conflict(new UserMessageDTO(response.ErrorMessage));

        if (response.Result == UserResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Registration Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Unknown Error"
        );
    }

    public async Task<Results<Ok<UserEntityDTO>, NotFound<UserMessageDTO>, BadRequest<UserMessagesDTO>, ProblemHttpResult>> Login(UserLoginDTO userDto, UserService service, IValidator<UserLoginDTO> validator)
    {
        var result = await validator.ValidateAsync(userDto);

        if (!result.IsValid)
            return TypedResults.BadRequest(new UserMessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        UserResponse response = await service.GetUserByLoginAsync(userDto);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Registration Failed: response is null"
            );

        if (response.Result == UserResultEnum.Success && response.User != null)
            return TypedResults.Ok(response.User);

        if (response.Result == UserResultEnum.UserNotFound && response.ErrorMessage != null)
            return TypedResults.NotFound(new UserMessageDTO(response.ErrorMessage));

        if (response.Result == UserResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Login Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Unknown Error"
        );
    }
}
