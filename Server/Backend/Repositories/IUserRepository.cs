using Backend.Models;

namespace Backend.Repositories;

public interface IUserRepository
{
    Task<User[]> GetAllUsersAsync();
    Task<User?> GetUserByEmailAsync(string email);
}
