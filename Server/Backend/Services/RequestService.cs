using Backend.Repositories.Interfaces;
using Shared.DTOs.Request;

namespace Backend.Services;

public enum RequestResultEnum
{
    Success,
    Failed,
    UserNotFound
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
                _ => null
            }
        };
}

public class RequestService(IRequestRepository requestRepository, IUserRepository userRepository)
{
    private readonly IRequestRepository _requestRepository = requestRepository;
    private readonly IUserRepository _userRepository = userRepository;

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

        if (requests == null)
            return RequestResponse.Success([]);

        return RequestResponse.Success([.. requests.Select(r => new RequestDTO(
            r.Email,
            r.DatetimeStart.ToString(),
            r.DatetimeEnd.ToString(),
            r.Kw,
            r.Paid,
            r.SlotId
        ))]);
    }
}
