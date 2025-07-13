namespace Shared.DTOs.Admin;

public record HistoryDTO(
    string Email,
    string DatetimeStart,
    string DatetimeEnd,
    int? Kw,
    int SlotId,
    string Type
);
