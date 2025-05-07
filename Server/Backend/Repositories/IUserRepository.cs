using Backend.Models;

namespace Backend.Repositories;

public interface IUserRepository
{
    Task<bool> CheckUserIfExistsByEmailAsync(string email);
    Task<bool> InsertUserAsync(string email, string password, string name, string surname);
    Task<User?> GetUserByEmailAndPasswordAsync(string email, string password);
}
