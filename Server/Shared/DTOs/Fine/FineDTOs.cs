namespace Shared.DTOs.Fine;

public record FineEntityDTO(
    string Email,
    string DatetimeStart,
    string DatetimeEnd,
    int Kw,
    bool Paid
);

public record class FineNewDTO()
{
    public string Email { get; set; } = string.Empty;
    public string DatetimeStart { get; set; } = string.Empty;
    public string DatetimeEnd { get; set; } = string.Empty;
    public int Kw { get; set; } = 0;
}
