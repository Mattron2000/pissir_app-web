using System.Globalization;
using Backend.Data;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Request;

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

        if (requests.Length != 1)
            return null;

        if (requests[0].Paid != null && requests[0].Paid == true)
            return null;

        requests[0].Paid = true;

        await _context.SaveChangesAsync();

        return requests;
    }

    public async Task<Request> AddRequestAsync(NewRequestDTO requestDto, int id)
    {
        var request = new Request
        {
            Email = requestDto.Email,
            DatetimeStart = DateTime.Parse(requestDto.DatetimeStart, new CultureInfo("it-IT")),
            DatetimeEnd = DateTime.Parse(requestDto.DatetimeEnd, new CultureInfo("it-IT")),
            SlotId = id
        };

        _context.Requests.Add(request);

        await _context.SaveChangesAsync();

        return request;
    }

    public async Task<Request?> DeleteRequestAsync(string email, DateTime datetime_start)
    {
        var request = await _context.Requests.FindAsync(email, datetime_start);

        if (request == null)
            return null;

        _context.Requests.Remove(request);
        await _context.SaveChangesAsync();

        return request;
    }

    public Task SetKwToRequestBySlotIdAsync(int slotId, int kw)
    {
        var request = _context.Requests.First(r => r.SlotId == slotId && r.Paid == false);
        request.Kw = kw;
        return _context.SaveChangesAsync();
    }
}
