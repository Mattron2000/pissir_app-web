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

    public static AdminResponse Success(PriceDataDTO[] prices) =>
        new()
        {
            Result = AdminResultEnum.Success,
            PricesData = prices
        };

    public static AdminResponse Success(HistoryDTO[] historyDTOs) =>
        new()
        {
            Result = AdminResultEnum.Success,
            History = historyDTOs
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
        // WHERE date(datetime_start) BETWEEN date_start AND date_end AND
        //       date(datetime_end)   BETWEEN date_start AND date_end
        if (date_start != null && date_end != null)
        {
            DateTime start = DateTime.Parse(date_start);
            DateTime end = DateTime.Parse(date_end);

            history = [.. history.Where(r =>
                r.DatetimeStart.Date >= start.Date &&
                r.DatetimeEnd.Date   <= end.Date
            )];
        }

        // SELECT email, type, datetime_start, datetime_end, kw, slot_id
        // FROM requests JOIN users USING (email)
        // WHERE time(datetime_start) BETWEEN time(time_start) AND time(time_end) OR
        //       time(datetime_end)   BETWEEN time(time_start) AND time(time_end)
        if (time_start != null && time_end != null)
        {
            TimeOnly start = TimeOnly.Parse(time_start);
            TimeOnly end = TimeOnly.Parse(time_end);

            history = [.. history.Where(r =>
                (TimeOnly.FromDateTime(r.DatetimeStart) >= start &&
                TimeOnly.FromDateTime(r.DatetimeStart) <= end) ||
                (TimeOnly.FromDateTime(r.DatetimeEnd)   >= start &&
                TimeOnly.FromDateTime(r.DatetimeEnd)   <= end)
            )];
        }

        if (service_type != null)
            switch (service_type)
            {
                case "PARKING":
                    history = [.. history.Where(r => r.Kw == null)];
                    break;
                case "CHARGING":
                    history = [.. history.Where(r => r.Kw != null)];
                    break;
            }

        if (user_type != null)
            switch (user_type)
            {
                case "BASE":
                    history = [.. history.Where(r => r.EmailNavigation.Type == UsersTypeEnum.BASE.ToString())];
                    break;
                case "PREMIUM":
                    history = [.. history.Where(r => r.EmailNavigation.Type == UsersTypeEnum.PREMIUM.ToString())];
                    break;
            }

        if (history == null)
            return AdminResponse.Failed(AdminResultEnum.Failed, "History is null");

        if (history.Length == 0)
            return AdminResponse.Success(Array.Empty<HistoryDTO>());

        return AdminResponse.Success(
            [.. history
                .Select(r => new HistoryDTO(
                    r.EmailNavigation.Email,
                    r.DatetimeStart.ToString(),
                    r.DatetimeEnd.ToString(),
                    r.Kw,
                    r.SlotId,
                    r.EmailNavigation.Type
                ))
            ]
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
