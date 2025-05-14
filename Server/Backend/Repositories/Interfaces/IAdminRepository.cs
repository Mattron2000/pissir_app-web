using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<Price[]> GetPricesAsync();
    Task<Price?> SetPriceAsync(string type, decimal amount);
}
