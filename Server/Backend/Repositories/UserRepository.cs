using Backend.Data;
using Backend.Models;

namespace Backend.Repositories;

public class UserRepository(SmartParkingContext context) : IUserRepository
{
    private readonly SmartParkingContext Context = context;

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await Context.Users.FindAsync(email);
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

        Context.Users.Add(user);

        try
        {
            var changes = await Context.SaveChangesAsync();
            return changes == 1;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SetUserTypeAsync(string email, string type)
    {
        var user = await Context.Users.FindAsync(email);

        if (user == null)
            return false;

        user.Type = type;

        return await Context.SaveChangesAsync() == 1;
    }
}
