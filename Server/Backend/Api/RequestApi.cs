using Backend.Services;

namespace Backend.Api;

public class RequestApi : IApiEndpoint
{
    private readonly string RootRequestApiV1 = "/api/v1/requests";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup(RootRequestApiV1));
    }

    private void MapV1(RouteGroupBuilder adminApi)
    {
        adminApi.MapGet("/{email}", (
            string email,
            bool? paid
        ) => GetUserRequest);
        adminApi.MapPatch("/{email}", UpdateUserRequest);
    }

    private Task GetUserRequest(string email, bool? paid, RequestService service)
    {
        throw new NotImplementedException();
    }

    private Task UpdateUserRequest(string email, RequestService service)
    {
        throw new NotImplementedException();
    }
}
