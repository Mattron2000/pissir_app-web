
using Backend.Models;
using Backend.Repositories.Interfaces;
using Shared.DTOs.Reservation;

namespace Backend.Services;

public enum ReservationResultEnum
{
    Success,
    Failed
}

public class ReservationResponse
{
    public ReservationResultEnum Result { get; set; }
    public string? ErrorMessage { get; set; }
    public ReservationEntityDTO[]? Reservations { get; internal set; }

    public static ReservationResponse Success(ReservationEntityDTO[] reservations) =>
        new()
        {
            Result = ReservationResultEnum.Success,
            Reservations = reservations
        };

    public static ReservationResponse Failed(ReservationResultEnum result = ReservationResultEnum.Failed, string? reason = null) =>
        new()
        {
            Result = result,
            ErrorMessage = reason ?? result switch
            {
                ReservationResultEnum.Failed => "Failed",
                _ => null
            }
        };
}

public class ReservationService(IReservationRepository repository)
{
    private readonly IReservationRepository _repository = repository;

    internal Task<ReservationResponse> CreateReservationAsync(ReservationCreateDTO reservation)
    {
        throw new NotImplementedException();
    }

    internal async Task<ReservationResponse> GetUserReservationsAsync(string email)
    {
        Reservation[]? reservations = await _repository.GetUserReservationsAsync(email);

        if (reservations == null)
            return ReservationResponse.Failed();

        return ReservationResponse.Success(
            [.. reservations.Select(r => new ReservationEntityDTO(
                r.Email,
                r.SlotId,
                r.DatetimeStart,
                r.DatetimeEnd
            ))]
        );
    }
}
