using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;

namespace Backend.Api;

public class UserApi : IApiEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup("/api/v1/users"));
    }

    private static void MapV1(RouteGroupBuilder userApi)
    {
        userApi.MapGet("/", GetAllUsers);
    }

    private static async Task<Ok<UserDTO[]>> GetAllUsers(UserService service)
    {
        return TypedResults.Ok(
            await service.GetAllUsersAsync()
        );
    }
}
