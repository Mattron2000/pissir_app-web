using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs.Slot;

namespace Backend.Api;

public class SlotApi : IApiEndpoint
{
    private readonly string RootSlotApiV1 = "/api/v1/slots";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup(RootSlotApiV1));
    }

    private void MapV1(RouteGroupBuilder slotApi)
    {
        slotApi.MapGet("/", GetSlots);
    }

    private async Task<
        Results<
            Ok<SlotEntityDTO[]>,
            ProblemHttpResult
        >
    > GetSlots(SlotService service)
    {
        SlotResponse response = await service.GetSlotsAsync();

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Get Slots Failed: response is null"
            );

        if (response.Result == SlotResultEnum.Success && response.Slots != null)
            return TypedResults.Ok(response.Slots);

        if (response.Result == SlotResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Get Slots Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Get Slots Failed with unknown error"
        );
    }
}
