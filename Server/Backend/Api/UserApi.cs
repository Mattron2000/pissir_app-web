using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;

namespace Backend.Api;

internal interface IUserApi
{
    Task<Results<Created<UserEntityDTO>, Conflict<UserMessageDTO>, BadRequest<UserMessageDTO>, ProblemHttpResult>> Register(UserRegisterDTO user, UserService service);
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
    }

    public async Task<Results<Created<UserEntityDTO>, Conflict<UserMessageDTO>, BadRequest<UserMessageDTO>, ProblemHttpResult>> Register(UserRegisterDTO userDto, UserService service)
    {
        UserResponse response = await service.CreateUserByRegistrationAsync(userDto);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Registration Failed: response is null"
            );

        if (response.Result == UserResultEnum.Success && response.User != null)
            return TypedResults.Created($"/api/v1/users/{response.User.Email}", response.User);

        if (response.Result == UserResultEnum.BadRequest && response.ErrorMessage != null)
            return TypedResults.BadRequest(new UserMessageDTO(response.ErrorMessage));

        if (response.Result == UserResultEnum.UserAlreadyExists && response.ErrorMessage != null)
            return TypedResults.Conflict(new UserMessageDTO(response.ErrorMessage));

        if (response.Result == UserResultEnum.UserInsertFailed && response.ErrorMessage != null)
            return TypedResults.BadRequest(new UserMessageDTO(response.ErrorMessage));

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
}
