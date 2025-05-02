using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api;

public class UserApi : IApiEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup("/api/v1/users"));
    }

    private void MapV1(RouteGroupBuilder userApi)
    {
        userApi.MapGet("/", GetAllUsers);
    }

    private static async Task<Ok<User[]>> GetAllUsers(SmartParkingContext context)
    {
        var users = await context.Users.ToArrayAsync();

        return TypedResults.Ok(users);
    }
}
