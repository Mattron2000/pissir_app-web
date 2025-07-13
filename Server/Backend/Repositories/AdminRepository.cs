using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class AdminRepository(SmartParkingContext context) : IAdminRepository
{
    private readonly SmartParkingContext _context = context;

    public async Task<Price[]> GetPricesAsync()
    {
        return await _context.Prices.ToArrayAsync();
    }

    public async Task<Price?> SetPriceAsync(string type, decimal amount)
    {
        var price = await _context.Prices.FindAsync(type);

        if (price == null)
            return null;

        price.Amount = amount;
        if (await _context.SaveChangesAsync() == 1)
            return price;

        return null;
    }

    public async Task<Request[]> GetRequestHistoryAsync()
    {
        return await _context.Requests
            .Include(r => r.EmailNavigation)
            .ToArrayAsync();
    }
}
