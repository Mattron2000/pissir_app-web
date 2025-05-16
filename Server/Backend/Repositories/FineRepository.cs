using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Fine;

namespace Backend.Repositories;

public class FineRepository(SmartParkingContext _context) : IFineRepository
{
    private readonly SmartParkingContext _context = _context;

    public async Task<Fine?> AddFineAsync(FineNewDTO fineDTO)
    {
        if (fineDTO == null)
            return null;

        var fine = new Fine
        {
            Email = fineDTO.Email,
            DatetimeStart = DateTime.Parse(fineDTO.DatetimeStart),
            DatetimeEnd = DateTime.Parse(fineDTO.DatetimeEnd),
            Paid = false
        };

        _context.Fines.Add(fine);

        try
        {
            var changes = await _context.SaveChangesAsync();
            return changes == 1 ? fine : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Fine[]?> GetFinesAsync()
    {
        return await _context.Fines.ToArrayAsync();
    }

    public async Task<Fine[]?> GetUserFinesAsync(string email)
    {
        return await _context.Fines.Where(f => f.Email == email).ToArrayAsync();
    }

    public async Task<Fine?> GetUserFineAsync(string email, string datetime)
    {
        return await _context.Fines.FindAsync(email, DateTime.Parse(datetime));
    }

    public async Task<Fine?> UpdateFineAsync(string email, string datetime)
    {
        var fine = await _context.Fines.FindAsync(email, DateTime.Parse(datetime));

        if (fine == null)
            return null;

        fine.Paid = true;

        return await _context.SaveChangesAsync() == 1 ? fine : null;
    }
}
