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

        adminApi.MapGet("/history", SearchHistory);
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
        AdminService service, PriceValidator priceValidator, PriceTypeValidator priceTypeValidator)
    {
        var result = await priceValidator.ValidateAsync(price);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        result = await priceTypeValidator.ValidateAsync(type);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

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

    private async Task<IResult> SearchHistory(
        string? date_start,
        string? date_end,
        string? time_start,
        string? time_end,
        string? user_type,
        string? service_type,
        AdminService service,
        DateValidator dateValidator,
        TimeValidator timeValidator,
        UserTypeValidator userTypeValidator,
        PriceTypeValidator serviceTypeValidator)
    {
        List<string> messages = [];

        if (date_start != null)
        {
            var result = await dateValidator.ValidateAsync(date_start);

            if (!result.IsValid)
                messages.AddRange([.. result.Errors.Select(e => e.ErrorMessage)]);
        }

        if (date_end != null)
        {
            var result = await dateValidator.ValidateAsync(date_end);

            if (!result.IsValid)
                messages.AddRange([.. result.Errors.Select(e => e.ErrorMessage)]);
        }

        if (time_start != null)
        {
            var result = await timeValidator.ValidateAsync(time_start);

            if (!result.IsValid)
                messages.AddRange([.. result.Errors.Select(e => e.ErrorMessage)]);
        }

        if (time_end != null)
        {
            var result = await timeValidator.ValidateAsync(time_end);

            if (!result.IsValid)
                messages.AddRange([.. result.Errors.Select(e => e.ErrorMessage)]);
        }

        if (user_type != null)
        {
            var result = await userTypeValidator.ValidateAsync(user_type);

            if (!result.IsValid)
                messages.AddRange([.. result.Errors.Select(e => e.ErrorMessage)]);
        }

        if (service_type != null)
        {
            var result = await serviceTypeValidator.ValidateAsync(service_type);

            if (!result.IsValid)
                messages.AddRange([.. result.Errors.Select(e => e.ErrorMessage)]);
        }

        if (date_start == null && date_end != null || date_start != null && date_end == null)
            messages.Add("Both start and end dates must be provided or omitted together.");

        if (date_start != null && date_end != null && DateTime.Parse(date_start) > DateTime.Parse(date_end))
            messages.Add("Start date must be before end date.");

        if (time_start != null && time_end != null && DateTime.Parse(time_start) > DateTime.Parse(time_end))
            messages.Add("Start time must be before end time.");

        if(messages.Count > 0)
            return TypedResults.BadRequest(new MessagesDTO([.. messages]));

        AdminResponse response = await service.SearchHistoryAsync(
            date_start,
            date_end,
            time_start,
            time_end,
            user_type,
            service_type
        );

        if (response.Result == AdminResultEnum.Success && response.History != null)
            return TypedResults.Ok(response.History);

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Search History Failed",
            detail: response.ErrorMessage
        );
    }
}
