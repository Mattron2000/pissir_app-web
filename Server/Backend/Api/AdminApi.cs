using Backend.Models;
using Backend.Services;

namespace Backend.Api;

enum UserTypeEnum
{
    BASE,
    PREMIUM
}

public class AdminApi : IApiEndpoint
{
    private readonly string RootAdminApiV1 = "/api/v1/admin";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup(RootAdminApiV1));
    }

    private void MapV1(RouteGroupBuilder adminApi)
    {
        adminApi.MapGet("/history", (
            DateOnly? date_start,
            DateOnly? date_end,
            TimeOnly? time_start,
            TimeOnly? time_end,
            UserTypeEnum? user_type,
            PricesTypeEnum? service_type
        ) => SearchHistory);

        adminApi.MapGet("/prices", () => GetPrices);
        adminApi.MapPatch("/prices", (
            PricesTypeEnum type,
            float price
        ) => SetPrice);
    }

    private Task<IResult> SearchHistory(
        DateOnly? date_start,
        DateOnly? date_end,
        TimeOnly? time_start,
        TimeOnly? time_end,
        UserTypeEnum? user_type,
        PricesTypeEnum? service_type,
        AdminService service)
    {
        throw new NotImplementedException();
    }

    private Task<IResult> GetPrices(AdminService service)
    {
        throw new NotImplementedException();
    }

    private Task<IResult> SetPrice(
        PricesTypeEnum type,
        float price,
        AdminService service)
    {
        throw new NotImplementedException();
    }
}
