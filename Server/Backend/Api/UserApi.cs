using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;

namespace Backend.Api;

public class UserApi : IApiEndpoint
{
    private string RootApiV1 { get; init; } = "/api/v1/users";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup($"{RootApiV1}"));
    }

    private void MapV1(RouteGroupBuilder userApi)
    {
        userApi.MapGet("/", GetAllUsers);
        userApi.MapGet("/{email}", GetUserByEmail);
    }

    private async Task<Ok<UserDTO[]>> GetAllUsers(UserService service)
    {
        return TypedResults.Ok(
            await service.GetAllUsersAsync()
        );
    }

    private static async Task<Results<Ok<UserDTO>, NotFound>> GetUserByEmail(string email, UserService service)
    {
        UserDTO? user = await service.GetUserByEmailAsync(email);

        if (user == null)   return TypedResults.NotFound();
        else                return TypedResults.Ok(user);
    }
}
