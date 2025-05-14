using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IReservationRepository
{
    Task<Reservation[]?> GetUserReservationsAsync(string email);
}
