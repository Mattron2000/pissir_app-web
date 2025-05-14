using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;
using Shared.DTOs.Admin;
using Shared.FluentValidators.Properties;

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
        adminApi.MapGet("/prices", GetPrices);
        adminApi.MapPatch("/prices", SetPrice);

        // adminApi.MapGet("/history", (
        //     DateOnly? date_start,
        //     DateOnly? date_end,
        //     TimeOnly? time_start,
        //     TimeOnly? time_end,
        //     UserTypeEnum? user_type,
        //     PricesTypeEnum? service_type
        // ) => SearchHistory);
    }

    private async Task<Results<Ok<PriceDataDTO[]>, ProblemHttpResult>> GetPrices(AdminService service)
    {
        AdminResponse response = await service.GetPrices();

        if (response.Result == AdminResultEnum.Success && response.PricesData != null)
            return TypedResults.Ok(response.PricesData);

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Get Prices Failed"
        );
    }

    private async Task<Results<Ok<PriceDataDTO>,BadRequest<MessagesDTO>, ProblemHttpResult>> SetPrice(
        string type,
        decimal price,
        AdminService service, PriceValidator validator)
    {
        var result = await validator.ValidateAsync(price);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        if (type != "PARKING" && type != "CHARGING")
            return TypedResults.BadRequest(new MessagesDTO(["Type must be PARKING or CHARGING"]));

        AdminResponse response = await service.SetPriceAsync(type, price);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Price Update Failed: response is null"
            );

        if (response.Result == AdminResultEnum.Success && response.PricesData != null && response.PricesData.Length == 1)
            return TypedResults.Ok(response.PricesData[0]);

        return TypedResults.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Price Validation Failed",
            detail: string.Join("\n", result.Errors.Select(e => e.ErrorMessage))
        );
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
}
