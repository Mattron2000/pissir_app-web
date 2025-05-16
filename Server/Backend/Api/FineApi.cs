using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;
using Shared.DTOs.Fine;
using Shared.FluentValidators.Properties;

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

    private async Task<
        Results<
            Ok<FineEntityDTO[]>,
            ProblemHttpResult
        >
    > GetFines(FineService service)
    {
        FineResponse response = await service.GetFinesAsync();

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Get Fines Failed: response is null"
            );

        if (response.Result == FineResultEnum.Success && response.Fines != null)
            return TypedResults.Ok(response.Fines);

        if (response.Result == FineResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Get Fines Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Get Fines Failed with unknown error"
        );
    }

    private async Task<
        Results<
        Ok<FineEntityDTO[]>,
        BadRequest<MessagesDTO>,
        ProblemHttpResult
        >
    > GetUserFines(
        string email,
        FineService service,
        EmailValidator validator)
    {
        var result = await validator.ValidateAsync(email);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        FineResponse response = await service.GetUserFinesAsync(email);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Get User Fines Failed: response is null"
            );

        if (response.Result == FineResultEnum.Success && response.Fines != null)
            return TypedResults.Ok(response.Fines);

        if (response.Result == FineResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Get User Fines Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Get User Fines Failed with unknown error"
        );
    }

    private async Task<
        Results<
            Created<FineEntityDTO>,
            BadRequest<MessageDTO>,
            BadRequest<MessagesDTO>,
            ProblemHttpResult
        >
    > AddFine(
        FineNewDTO fine,
        FineService fineService,
        UserService userService,
        IValidator<FineNewDTO> validator)
    {
        if (fine == null)
            return TypedResults.BadRequest(new MessageDTO("Fine body is null"));

        var result = await validator.ValidateAsync(fine);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        if (DateTime.Parse(fine.DatetimeStart) > DateTime.Parse(fine.DatetimeEnd))
            return TypedResults.BadRequest(new MessageDTO("Datetime start is after datetime end"));

        UserResponse userResponse = await userService.GetUserByEmailAsync(fine.Email);

        if (userResponse == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Add Fine Failed: user response is null"
            );

        if (userResponse.Result == UserResultEnum.UserNotFound && userResponse.ErrorMessage != null)
            return TypedResults.BadRequest(new MessageDTO(userResponse.ErrorMessage));

        FineResponse response = await fineService.AddFineAsync(fine);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Add Fine Failed: response is null"
            );

        if (response.Result == FineResultEnum.Success && response.Fines != null)
            return TypedResults.Created($"/api/v1/fines/{response.Fines[0].Email}", response.Fines[0]);

        if (response.Result == FineResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Add Fine Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Add Fine Failed with unknown error"
        );
    }

    private async Task<
        Results<
            Ok<FineEntityDTO>,
            BadRequest<MessageDTO>,
            BadRequest<MessagesDTO>,
            ProblemHttpResult
        >
    > UpdateFine(string email, string datetime, FineService service, EmailValidator emailValidator, DateTimeValidator datetimeValidator)
    {
        List<string> messages = [];

        var result = await emailValidator.ValidateAsync(email);

        if (!result.IsValid)
            messages.AddRange([.. result.Errors.Select(e => e.ErrorMessage)]);

        result = await datetimeValidator.ValidateAsync(datetime);

        if (!result.IsValid)
            messages.AddRange([.. result.Errors.Select(e => e.ErrorMessage)]);

        if (messages.Count > 0)
            return TypedResults.BadRequest(new MessagesDTO([.. messages]));

        FineResponse response = await service.UpdateFineAsync(email, datetime);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Update Fine Failed: response is null"
            );

        if (response.Result == FineResultEnum.Success && response.Fines != null)
            return TypedResults.Ok(response.Fines.First());

        if (response.Result == FineResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Update Fine Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Update Fine Failed with unknown error"
        );
    }
}
