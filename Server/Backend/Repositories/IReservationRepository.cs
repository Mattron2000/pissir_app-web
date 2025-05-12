using Backend.Models;

namespace Backend.Repositories;

public interface IReservationRepository
{
    Task<Reservation[]?> GetUserReservationsAsync(string email);
}
