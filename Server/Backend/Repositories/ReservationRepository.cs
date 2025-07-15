using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Reservation;

namespace Backend.Repositories;

public class ReservationRepository(SmartParkingContext context) : IReservationRepository
{
    private readonly SmartParkingContext _context = context;

    public async Task<bool> CreateReservationAsync(ReservationCreateDTO reservation)
    {
        _context.Reservations.Add(new Reservation
        {
            Email = reservation.Email,
            SlotId = reservation.SlotId,
            DatetimeStart = reservation.DatetimeStart,
            DatetimeEnd = reservation.DatetimeEnd
        });
        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<bool> DeleteReservationAsync(Reservation reservation)
    {
        _context.Reservations.Remove(reservation);
        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<Reservation[]?> GetUserReservationsAsync(string email)
    {
        return await _context.Reservations.Where(r => r.Email == email).ToArrayAsync();
    }
}
