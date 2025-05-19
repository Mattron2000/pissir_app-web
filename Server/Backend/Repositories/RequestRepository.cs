using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class RequestRepository(SmartParkingContext context) : IRequestRepository
{
    private readonly SmartParkingContext _context = context;

    public async Task<Request[]?> GetRequestsAsync(string email)
    {
        return await _context.Requests.Where(r => r.Email == email).ToArrayAsync();
    }

    public async Task<Request[]?> UpdateRequestsAsync(string email)
    {
        var requests = await _context.Requests.Where(
            r => r.Email == email && r.Paid == false
        ).ToArrayAsync();

        foreach (var request in requests) request.Paid = true;

        await _context.SaveChangesAsync();

        return requests;
    }
}
