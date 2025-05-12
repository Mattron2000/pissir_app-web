using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class ReservationRepository(SmartParkingContext context) : IReservationRepository
{
    private readonly SmartParkingContext _context = context;

    public async Task<Reservation[]?> GetUserReservationsAsync(string email)
    {
        return await _context.Reservations.Where(r => r.Email == email).ToArrayAsync();
    }
}
