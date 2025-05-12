using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;
using Shared.DTOs.User;
using Shared.FluentValidators;

namespace Backend.Api;

public class UserApi : IApiEndpoint
{
    private readonly string RootUserApiV1 = "/api/v1/users";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup(RootUserApiV1));
    }

    private void MapV1(RouteGroupBuilder userApi)
    {
        userApi.MapPost("/register", Register);
        userApi.MapPost("/login", Login);
        userApi.MapPatch("/{email}/type", SwitchUserType);
    }

    private async Task<Results<Created<UserEntityDTO>, Conflict<MessageDTO>, BadRequest<MessagesDTO>, ProblemHttpResult>> Register(UserRegisterDTO userDto, UserService service, IValidator<UserRegisterDTO> validator)
    {
        var result = await validator.ValidateAsync(userDto);
        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        UserResponse response = await service.CreateUserByRegistrationAsync(userDto);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Registration Failed: response is null"
            );

        if (response.Result == UserResultEnum.Success && response.User != null)
            return TypedResults.Created($"/api/v1/users/{response.User.Email}", response.User);

        if (response.Result == UserResultEnum.UserAlreadyExists && response.ErrorMessage != null)
            return TypedResults.Conflict(new MessageDTO(response.ErrorMessage));

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

    private async Task<Results<Ok<UserEntityDTO>, NotFound<MessageDTO>, BadRequest<MessagesDTO>, ProblemHttpResult>> Login(UserLoginDTO userDto, UserService service, IValidator<UserLoginDTO> validator)
    {
        var result = await validator.ValidateAsync(userDto);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        UserResponse response = await service.GetUserByLoginAsync(userDto);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Login Failed: response is null"
            );

        if (response.Result == UserResultEnum.Success && response.User != null)
            return TypedResults.Ok(response.User);

        if (response.Result == UserResultEnum.UserNotFound && response.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(response.ErrorMessage));

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

    private async Task<Results<Ok<UserEntityDTO>, NotFound<MessageDTO>, BadRequest<MessagesDTO>, ProblemHttpResult>> SwitchUserType(string email, UserService service, EmailValidator validator)
    {
        var result = await validator.ValidateAsync(email);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        UserResponse response = await service.SwitchUserTypeAsync(email);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Update Failed: response is null"
            );

        if (response.Result == UserResultEnum.Success && response.User != null)
            return TypedResults.Ok(response.User);

        if (response.Result == UserResultEnum.UserNotFound && response.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(response.ErrorMessage));

        if (response.Result == UserResultEnum.Forbid && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "User Type Update Forbidden",
                detail: response.ErrorMessage
            );

        if (response.Result == UserResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Type Update Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Unknown Error"
        );
    }
}
