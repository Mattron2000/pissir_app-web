
using Backend.Models;
using Backend.Repositories.Interfaces;
using Shared.DTOs.Reservation;

namespace Backend.Services;

public enum ReservationResultEnum
{
    Success,
    Failed,
    NotFound
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
                ReservationResultEnum.NotFound => "Reservation not found",
                _ => null
            }
        };
}

public class ReservationService(IReservationRepository ReservationRepository, ISlotRepository SlotRepository)
{
    private readonly IReservationRepository _reservationRepository = ReservationRepository;
    private readonly ISlotRepository _slotRepository = SlotRepository;

    internal async Task<ReservationResponse> CreateReservationAsync(ReservationCreateDTO reservation)
    {
        if (reservation.SlotId == 0)
        {
            var slots = await _slotRepository.GetSlotsAsync();

            if (slots == null)
                return ReservationResponse.Failed();

            var avaiableSlots = slots.Select(s => s.Status = SlotsStatusEnum.FREE.ToString()).ToArray();

            if (avaiableSlots.Length == 0)
                return ReservationResponse.Failed(ReservationResultEnum.Failed, "No free slots");

            reservation.SlotId = slots[new Random().Next(slots.Length)].Id;
        }

        if (await _reservationRepository.CreateReservationAsync(reservation))
            return ReservationResponse.Success([new ReservationEntityDTO(
                reservation.Email,
                reservation.SlotId,
                reservation.DatetimeStart,
                reservation.DatetimeEnd
            )]);
        else
            return ReservationResponse.Failed(ReservationResultEnum.Failed,"Failed to create reservation");
    }

    internal async Task<ReservationResponse> DeleteReservationAsync(string email, DateTime datetimeStart)
    {
        var reservations = await _reservationRepository.GetUserReservationsAsync(email);

        if (reservations == null)
            return ReservationResponse.Failed();

        var reservation = reservations.FirstOrDefault(r => r.DatetimeStart.ToString("yyyy-MM-dd HH:mm") == datetimeStart.ToString("yyyy-MM-dd HH:mm"));

        if (reservation == null)
            return ReservationResponse.Failed(ReservationResultEnum.NotFound);

        if (await _reservationRepository.DeleteReservationAsync(reservation))
            return ReservationResponse.Success([new ReservationEntityDTO(
                reservation.Email,
                reservation.SlotId,
                reservation.DatetimeStart,
                reservation.DatetimeEnd
            )]);
        else
            return ReservationResponse.Failed(ReservationResultEnum.Failed, "Failed to delete reservation");
    }

    internal async Task<ReservationResponse> GetUserReservationsAsync(string email)
    {
        Reservation[]? reservations = await _reservationRepository.GetUserReservationsAsync(email);

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
