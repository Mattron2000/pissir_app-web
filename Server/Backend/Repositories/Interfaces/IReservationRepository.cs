using Backend.Models;
using Shared.DTOs.Reservation;

namespace Backend.Repositories.Interfaces;

public interface IReservationRepository
{
    Task<bool> CreateReservationAsync(ReservationCreateDTO reservation);
    Task<bool> DeleteReservationAsync(Reservation reservation);
    Task<Reservation[]?> GetUserReservationsAsync(string email);
}
