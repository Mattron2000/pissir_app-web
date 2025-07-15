using Backend.Models;
using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;
using Shared.DTOs.Reservation;
using Shared.FluentValidators.Properties;

namespace Backend.Api;

public class ReservationApi : IApiEndpoint
{
    private readonly string RootReservationApiV1 = "/api/v1/reservations";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        MapV1(app.MapGroup(RootReservationApiV1));
    }

    private void MapV1(RouteGroupBuilder reservationApi)
    {
        reservationApi.MapPost("/", CreateReservation);
        reservationApi.MapGet("/{email}", GetUserReservations);
        reservationApi.MapDelete("/", DeleteReservation);
    }

    private async Task<
        Results<
            BadRequest<MessageDTO>,
            BadRequest<MessagesDTO>,
            NotFound<MessageDTO>,
            Created<ReservationEntityDTO>,
            ProblemHttpResult
        >
    > CreateReservation(
        ReservationCreateDTO reservation,
        UserService userService,
        ReservationService reservationService,
        IValidator<ReservationCreateDTO> validator)
    {
        if (reservation == null)
            return TypedResults.BadRequest(new MessageDTO("Reservation body is null"));

        var result = await validator.ValidateAsync(reservation);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        UserResponse userResponse = await userService.GetUserByEmailAsync(reservation.Email);

        if (userResponse == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Reservation Creation Failed: user response is null"
            );

        if (userResponse.Result == UserResultEnum.UserNotFound && userResponse.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(userResponse.ErrorMessage));

        if (userResponse.Result == UserResultEnum.Success &&
                userResponse.User != null &&
                userResponse.User.Type != UsersTypeEnum.PREMIUM.ToString())
            return TypedResults.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Reservation Creation Forbidden for non-premium users"
            );

        ReservationResponse response = await reservationService.CreateReservationAsync(reservation);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Reservation Creation Failed: reservation response is null"
            );

        if (response.Result == ReservationResultEnum.Success && response.Reservations != null)
            return TypedResults.Created($"{RootReservationApiV1}/{response.Reservations[0].Email}", response.Reservations[0]);

        if (response.Result == ReservationResultEnum.Failed && response.ErrorMessage != null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Reservation Creation Failed",
                detail: response.ErrorMessage
            );

        return TypedResults.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Reservation Creation Failed with unknown error",
            detail: response.ErrorMessage
        );
    }

    private async Task<
        Results<
            BadRequest<MessagesDTO>,
            NotFound<MessageDTO>,
            ProblemHttpResult,
            Ok<ReservationEntityDTO[]>
        >
    > GetUserReservations(
        string email,
        UserService userService,
        ReservationService reservationService,
        EmailValidator validator)
    {
        var result = await validator.ValidateAsync(email);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        UserResponse userResponse = await userService.GetUserByEmailAsync(email);

        if (userResponse == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Reservations Failed: user response is null"
            );

        if (userResponse.Result == UserResultEnum.UserNotFound && userResponse.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(userResponse.ErrorMessage));

        if (userResponse.Result == UserResultEnum.Success &&
                userResponse.User != null &&
                userResponse.User.Type != UsersTypeEnum.PREMIUM.ToString())
            return TypedResults.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "User Reservations Forbidden for non-premium users"
            );

        ReservationResponse response = await reservationService.GetUserReservationsAsync(email);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Reservations Failed: reservation response is null"
            );

        if (response.Result == ReservationResultEnum.Success && response.Reservations != null)
            return TypedResults.Ok(response.Reservations);

        return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "User Reservations Failed",
                detail: response.ErrorMessage
            );
    }

    private async Task<
        Results<
            BadRequest<MessagesDTO>,
            NotFound<MessageDTO>,
            NoContent,
            ProblemHttpResult
        >
    > DeleteReservation(
        string email,
        DateTime datetime,
        UserService userService,
        ReservationService reservationService,
        EmailValidator validator)
    {
        var result = await validator.ValidateAsync(email);

        if (!result.IsValid)
            return TypedResults.BadRequest(new MessagesDTO([.. result.Errors.Select(e => e.ErrorMessage)]));

        Console.WriteLine($"email: {email}");
        Console.WriteLine($"datetimeStart: {datetime}");

        UserResponse userResponse = await userService.GetUserByEmailAsync(email);

        if (userResponse == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Delete Reservation Failed: user response is null"
            );

        if (userResponse.Result == UserResultEnum.UserNotFound && userResponse.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(userResponse.ErrorMessage));

        if (userResponse.Result == UserResultEnum.Success &&
                userResponse.User != null &&
                userResponse.User.Type != UsersTypeEnum.PREMIUM.ToString())
            return TypedResults.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Delete Reservation Forbidden for non-premium users"
            );

        ReservationResponse response = await reservationService.DeleteReservationAsync(email, datetime);

        if (response == null)
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Delete Reservation Failed: reservation response is null"
            );

        if (response.Result == ReservationResultEnum.Success)
            return TypedResults.NoContent();

        if (response.Result == ReservationResultEnum.NotFound && response.ErrorMessage != null)
            return TypedResults.NotFound(new MessageDTO(response.ErrorMessage));

        return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Delete Reservation Failed",
                detail: response.ErrorMessage
            );
    }
}
