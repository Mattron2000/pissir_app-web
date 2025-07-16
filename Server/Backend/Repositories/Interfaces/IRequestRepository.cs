using Backend.Models;
using Shared.DTOs.Request;

namespace Backend.Repositories.Interfaces;

public interface IRequestRepository
{
    Task<Request> AddRequestAsync(NewRequestDTO requestDto, int id);
    Task<Request?> DeleteRequestAsync(string email, DateTime datetime_start);
    Task<Request[]?> GetRequestsAsync(string email);
    Task SetKwToRequestBySlotIdAsync(int slotId, int kw);
    Task<Request[]?> UpdateRequestsAsync(string email);
}
