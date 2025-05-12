namespace Shared.DTOs.Reservation;

public record ReservationEntityDTO(
    string Email,
    int SlotId,
    DateTime DatetimeStart,
    DateTime DatetimeEnd
);
