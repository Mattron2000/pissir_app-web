using Backend.Services;

namespace Backend.Api;

public class FineApi : IApiEndpoint
{
    private readonly string RootFineApiV1 = "/api/v1/fines";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup(RootFineApiV1));
    }

    private void MapV1(RouteGroupBuilder adminApi)
    {
        adminApi.MapGet("/", GetFines);
        adminApi.MapGet("/{email}", GetUserFines);
        adminApi.MapPost("/", AddFine);
        adminApi.MapPatch("/", UpdateFine);
    }

    private Task GetFines(FineService service)
    {
        throw new NotImplementedException();
    }

    private Task GetUserFines(string email, FineService service)
    {
        throw new NotImplementedException();
    }

    private Task AddFine(FineService service)
    {
        throw new NotImplementedException();
    }

    private Task UpdateFine(FineService service)
    {
        throw new NotImplementedException();
    }
}
