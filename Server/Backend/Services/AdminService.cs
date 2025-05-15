using Backend.Models;
using Backend.Repositories.Interfaces;
using Shared.DTOs.Admin;

namespace Backend.Services;

public enum AdminResultEnum
{
    Success,
    Failed
}

public class AdminResponse
{
    public AdminResultEnum Result { get; init; }
    public PriceDataDTO[]? PricesData { get; init; }
    public string? ErrorMessage { get; init; }
    public HistoryDTO[]? History { get; init; }

    public static AdminResponse Success(
        PriceDataDTO[] prices
        ) =>
        new()
        {
            Result = AdminResultEnum.Success,
            PricesData = prices
        };

    public static AdminResponse Failed(AdminResultEnum result = AdminResultEnum.Failed, string? reason = null) =>
        new()
        {
            Result = result,
            ErrorMessage = reason ?? result switch
            {
                // AdminResultEnum.AdminAlreadyExists => "Admin already exists",
                // AdminResultEnum.AdminNotFound => "Admin not found",
                // AdminResultEnum.Forbid => "Forbidden",
                AdminResultEnum.Failed => "Failed",
                _ => null
            }
        };

    internal static AdminResponse Success(HistoryDTO[] historyDTOs)
    {
        return new()
        {
            Result = AdminResultEnum.Success,
            History = historyDTOs
        };
    }
}

public class AdminService(IAdminRepository repository)
{
    private readonly IAdminRepository _repository = repository;

    internal async Task<AdminResponse> GetPrices()
    {
        Price[] prices = await _repository.GetPricesAsync();

        if (prices == null)
            return AdminResponse.Failed(AdminResultEnum.Failed, "Prices not found");

        return AdminResponse.Success(
            [.. prices.Select(p => new PriceDataDTO(p.Type, p.Amount))]
        );
    }

    internal async Task<AdminResponse> SearchHistoryAsync(
        string? date_start,
        string? date_end,
        string? time_start,
        string? time_end,
        string? user_type,
        string? service_type)
    {
        Request[] history = await _repository.GetRequestHistoryAsync();

        history = [.. history.Where(r => r.Paid == true)];

        // SELECT email, type, datetime_start, datetime_end, kw, slot_id
        // FROM requests JOIN users USING (email)
        // WHERE date(datetime_start) BETWEEN date_start AND date_end AND date(datetime_end) BETWEEN date_start AND date_end
        if (date_start != null && date_end != null)
            history = [.. history.Where(r =>
                r.DatetimeStart.Date >= DateTime.Parse(date_start).Date &&
                r.DatetimeStart.Date <= DateTime.Parse(date_end).Date &&
                r.DatetimeEnd.Date   >= DateTime.Parse(date_start).Date &&
                r.DatetimeEnd.Date   <= DateTime.Parse(date_end).Date
            )];

        // SELECT email, type, datetime_start, datetime_end, kw, slot_id
        // FROM requests JOIN users USING (email)
        // WHERE (time(datetime_start) BETWEEN time(time_start) AND time(time_end) OR time(datetime_end) BETWEEN time(time_start) AND time(time_end))
        if (time_start != null && time_end != null)
            history = [.. history.Where(r =>
                r.DatetimeStart.TimeOfDay >= DateTime.Parse(time_start).TimeOfDay &&
                r.DatetimeStart.TimeOfDay <= DateTime.Parse(time_end).TimeOfDay &&
                r.DatetimeEnd.TimeOfDay   >= DateTime.Parse(time_start).TimeOfDay &&
                r.DatetimeEnd.TimeOfDay   <= DateTime.Parse(time_end).TimeOfDay
            )];

        if(service_type != null)
            if (service_type == "PARKING")
                history = [.. history.Where(r => r.Kw == null)];
            else
                history = [.. history.Where(r => r.Kw != null)];

        if(user_type != null)
            history = [.. history.Where(r => r.EmailNavigation.Type == user_type)];

        return AdminResponse.Success(
            history.Select(h => new HistoryDTO(
                h.Email,
                h.DatetimeStart.ToString(),
                h.DatetimeEnd.ToString(),
                h.Kw,
                h.SlotId,
                h.EmailNavigation.Type,
                h.EmailNavigation.Name,
                h.EmailNavigation.Surname
            )).ToArray()
        );
    }

    internal async Task<AdminResponse> SetPriceAsync(string type, decimal amount)
    {
        Price? price = await _repository.SetPriceAsync(type, amount);

        if (price == null)
            return AdminResponse.Failed(AdminResultEnum.Failed, "Price not found");

        return AdminResponse.Success(
            [new PriceDataDTO(price.Type, price.Amount)]
        );
    }
}
