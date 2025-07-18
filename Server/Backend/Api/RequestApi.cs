using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Request;
using Shared.FluentValidators.Properties;

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
        adminApi.MapGet("/{email}", GetUserRequest);
        adminApi.MapPatch("/{email}", UpdateUserRequest);
        adminApi.MapPost("/", AddRequest);
        adminApi.MapDelete("/", DeleteRequest);
    }

    private async Task<
        Results<
            BadRequest<MessagesDTO>,
            NotFound<MessageDTO>,
            ProblemHttpResult,
            Ok<RequestDTO>
        >
    > DeleteRequest(
        [FromQuery(Name = "email")] string email,
        [FromQuery(Name = "datetime_start")] DateTime datetime_start,
        RequestService service,
        EmailValidator validator)
    {
        var result = await validator.ValidateAsync(email);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        RequestResponse response = await service.DeleteRequestAsync(email, datetime_start);

        if (response == null)
            return TypedResults.NotFound(new MessageDTO("request not found"));

        if (response.Result == RequestResultEnum.Success && response.Requests != null && response.Requests.Length == 1)
            return TypedResults.Ok(response.Requests[0]);

        if (response.Result == RequestResultEnum.UserNotFound && response.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(response.ErrorMessage));

        if (response.Result == RequestResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Delete Request Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Delete Request is not implemented yet"
        );
    }

    private async Task<
        Results<
            BadRequest<MessagesDTO>,
            NotFound<MessageDTO>,
            ProblemHttpResult,
            Ok<RequestDTO[]>
        >
    > GetUserRequest(
        string email,
        bool? paid,
        RequestService service,
        EmailValidator validator)
    {
        var result = await validator.ValidateAsync(email);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        RequestResponse response = await service.GetRequestAsync(email, paid);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Request Failed: request response is null"
            );

        if (response.Result == RequestResultEnum.Success && response.Requests != null)
            return TypedResults.Ok(response.Requests);

        if (response.Result == RequestResultEnum.UserNotFound && response.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(response.ErrorMessage));

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "User Request Failed"
        );
    }

    private async Task<
        Results<
            Ok<RequestDTO[]>,
            NotFound<MessageDTO>,
            BadRequest<MessagesDTO>,
            ProblemHttpResult
        >
    > UpdateUserRequest(
        string email,
        RequestService service,
        EmailValidator validator)
    {
        var result = await validator.ValidateAsync(email);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        RequestResponse response = await service.UpdateRequestAsync(email);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status304NotModified,
                title: "User Request Update Failed: request response is null"
            );

        if (response.Result == RequestResultEnum.Success && response.Requests != null)
            return TypedResults.Ok(response.Requests);

        if (response.Result == RequestResultEnum.UserNotFound && response.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(response.ErrorMessage));

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "User Request Update Failed"
        );
    }

    private async Task<
        Results<
            Created<RequestDTO>,
            NotFound<MessageDTO>,
            BadRequest<MessagesDTO>,
            Conflict<MessageDTO>,
            ForbidHttpResult,
            ProblemHttpResult
        >
    > AddRequest(
        string? slot_id,
        NewRequestDTO requestDto,
        RequestService service,
        IValidator<NewRequestDTO> validator)
    {
        var result = await validator.ValidateAsync(requestDto);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        RequestResponse response = await service.AddRequestAsync(requestDto, slot_id);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Add Request Failed: request response is null"
            );

        if (response.Result == RequestResultEnum.UserNotFound && response.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(response.ErrorMessage));

        if (response.Result == RequestResultEnum.RequestAlreadyExists && response.ErrorMessage != null)
            return TypedResults.Conflict(new MessageDTO(response.ErrorMessage));

        if (response.Result == RequestResultEnum.Forbid && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Add Request Failed",
                detail: response.ErrorMessage
            );

        if (!(response.Result == RequestResultEnum.Success && response.Requests != null && response.Requests.Length == 1))
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Add Request Failed"
            );

        return TypedResults.Created("/api/v1/requests", response.Requests[0]);
    }
}
