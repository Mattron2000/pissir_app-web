using Backend.Models;
using Backend.Repositories.Interfaces;
using Shared.DTOs.Request;

namespace Backend.Services;

public enum RequestResultEnum
{
    Success,
    Failed,
    UserNotFound,
    RequestAlreadyExists
}

public class RequestResponse
{
    public RequestResultEnum Result { get; init; }
    public string? ErrorMessage { get; init; }
    public RequestDTO[]? Requests { get; init; }

    public static RequestResponse Success(
        RequestDTO[] requests
        ) =>
        new()
        {
            Result = RequestResultEnum.Success,
            Requests = requests
        };

    public static RequestResponse Failed(RequestResultEnum result = RequestResultEnum.Failed, string? reason = null) =>
        new()
        {
            Result = result,
            ErrorMessage = reason ?? result switch
            {
                RequestResultEnum.UserNotFound => "User not found",
                RequestResultEnum.RequestAlreadyExists => "Request already exists",
                RequestResultEnum.Failed => "Failed",
                _ => null
            }
        };
}

public class RequestService(IRequestRepository requestRepository, IUserRepository userRepository, ISlotRepository slotRepository)
{
    private readonly IRequestRepository _requestRepository = requestRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISlotRepository _slotRepository = slotRepository;

    internal async Task<RequestResponse> GetRequestAsync(string email, bool? paid)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null)
            return RequestResponse.Failed(RequestResultEnum.UserNotFound);

        var requests = await _requestRepository.GetRequestsAsync(email);

        if (requests == null)
            return RequestResponse.Failed();

        if (paid != null)
            requests = [.. requests.Where(r => r.Paid == paid)];

        return RequestResponse.Success([.. requests.Select(r => new RequestDTO(
            r.Email,
            r.DatetimeStart.ToString(),
            r.DatetimeEnd.ToString(),
            r.Kw,
            r.Paid,
            r.SlotId
        ))]);
    }

    internal async Task<RequestResponse> UpdateRequestAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null)
            return RequestResponse.Failed(RequestResultEnum.UserNotFound);

        var requests = await _requestRepository.UpdateRequestsAsync(email);

        if (requests == null || requests.Length != 1) return RequestResponse.Failed();

        return RequestResponse.Success([new RequestDTO(
            requests[0].Email,
            requests[0].DatetimeStart.ToString(),
            requests[0].DatetimeEnd.ToString(),
            requests[0].Kw,
            requests[0].Paid,
            requests[0].SlotId
        )]);
    }

    internal async Task<RequestResponse> AddRequestAsync(NewRequestDTO requestDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(requestDto.Email);

        if (user == null)
            return RequestResponse.Failed(RequestResultEnum.UserNotFound);

        var requests = await _requestRepository.GetRequestsAsync(requestDto.Email);

        if (requests != null && requests.Length > 0)
            if (requests.Any(r => r.Paid == false))
                return RequestResponse.Failed(RequestResultEnum.RequestAlreadyExists, "Existing request not paid");

        Slot[] slots = await _slotRepository.GetSlotsAsync();

        slots = [.. slots.Where(s => s.Status == SlotsStatusEnum.FREE.ToString())];

        if (slots.Length == 0)
            return RequestResponse.Failed(RequestResultEnum.Failed, "No free slots");

        Slot selectedSlot = slots[new Random().Next(slots.Length)];

        Request request = await _requestRepository.AddRequestAsync(requestDto, selectedSlot.Id);

        return RequestResponse.Success([new RequestDTO(
            request.Email,
            request.DatetimeStart.ToString(),
            request.DatetimeEnd.ToString(),
            request.Kw,
            request.Paid,
            request.SlotId
        )]);
    }

    internal async Task<RequestResponse> DeleteRequestAsync(string email, DateTime datetime_start)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null)
            return RequestResponse.Failed(RequestResultEnum.UserNotFound);

        Request? request = await _requestRepository.DeleteRequestAsync(email, datetime_start);

        if (request == null)
            return RequestResponse.Failed();

        return RequestResponse.Success([new RequestDTO(
            request.Email,
            request.DatetimeStart.ToString(),
            request.DatetimeEnd.ToString(),
            request.Kw,
            request.Paid,
            request.SlotId
        )]);
    }
}
