using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class SlotRepository(SmartParkingContext context) : ISlotRepository
{
    private readonly SmartParkingContext _context = context;

    public async Task<Slot[]> GetSlotsAsync()
    {
        return await _context.Slots.ToArrayAsync();
    }
}
