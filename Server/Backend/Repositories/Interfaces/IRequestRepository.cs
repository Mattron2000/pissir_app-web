using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IRequestRepository
{
    Task<Request[]?> GetRequestsAsync(string email);
    Task<Request[]?> UpdateRequestsAsync(string email);
}
