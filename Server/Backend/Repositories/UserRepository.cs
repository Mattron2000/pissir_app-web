using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;

namespace Backend.Repositories;

public class UserRepository(SmartParkingContext context) : IUserRepository
{
    private readonly SmartParkingContext _context = context;

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FindAsync(email);
    }

    public async Task<bool> InsertUserAsync(string email, string password, string name, string surname)
    {
        var user = new User
        {
            Email = email,
            Password = password,
            Name = name,
            Surname = surname
        };

        _context.Users.Add(user);

        try
        {
            var changes = await _context.SaveChangesAsync();
            return changes == 1;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SetUserTypeAsync(string email, string type)
    {
        var user = await _context.Users.FindAsync(email);

        if (user == null)
            return false;

        user.Type = type;

        return await _context.SaveChangesAsync() == 1;
    }
}
