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
