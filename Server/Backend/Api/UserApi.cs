using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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

    private static async Task<Ok<UserDTO[]>> GetAllUsers(SmartParkingContext context)
    {
        UserDTO[] users = await context.Users.Select(user => new UserDTO(
            user.Email,
            user.Name,
            user.Surname,
            user.Type
        )).ToArrayAsync();

        return TypedResults.Ok(users);
    }
}
