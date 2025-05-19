namespace Shared.DTOs.Request;

public record RequestDTO(
    string Email,
    string DatetimeStart,
    string DatetimeEnd,
    int? Kw,
    bool? Paid,
    int SlotId
);
