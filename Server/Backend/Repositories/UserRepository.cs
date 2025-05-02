using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class UserRepository(SmartParkingContext context) : IUserRepository
{
    private SmartParkingContext _context { get; init; } = context;

    public async Task<User[]> GetAllUsersAsync()
    {
        return await _context.Users.ToArrayAsync();
    }
}
