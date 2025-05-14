using Backend.Models;

namespace Backend.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> InsertUserAsync(string email, string password, string name, string surname);
    Task<bool> SetUserTypeAsync(string email, string type);
}
