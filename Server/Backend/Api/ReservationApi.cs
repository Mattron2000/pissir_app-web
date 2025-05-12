using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.DTOs;
using Shared.DTOs.Reservation;
using Shared.FluentValidators;

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
        reservationApi.MapGet("/{email}", GetUserReservations);
    }

    private async Task<Results<BadRequest<MessagesDTO>, NotFound<MessageDTO>, ProblemHttpResult, Ok<ReservationEntityDTO[]>>> GetUserReservations(string email, UserService userService, ReservationService reservationService, EmailValidator validator)
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
}
