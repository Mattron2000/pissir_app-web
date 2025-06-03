using Backend.Models;
using Shared.DTOs.Request;

namespace Backend.Repositories.Interfaces;

public interface IRequestRepository
{
    Task<Request> AddRequestAsync(NewRequestDTO requestDto, int id);
    Task<Request[]?> GetRequestsAsync(string email);
    Task<Request[]?> UpdateRequestsAsync(string email);
}
