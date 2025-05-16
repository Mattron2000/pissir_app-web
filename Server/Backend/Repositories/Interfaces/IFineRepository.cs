using Backend.Models;
using Shared.DTOs.Fine;

namespace Backend.Repositories.Interfaces;

public interface IFineRepository
{
    Task<Fine?> AddFineAsync(FineNewDTO fine);
    Task<Fine[]?> GetFinesAsync();
    Task<Fine?> GetUserFineAsync(string email, string datetime);
    Task<Fine[]?> GetUserFinesAsync(string email);
    Task<Fine?> UpdateFineAsync(string email, string datetime);
}
