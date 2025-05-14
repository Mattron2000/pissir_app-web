using Backend.Repositories.Interfaces;

namespace Backend.Services;

public enum RequestResultEnum
{
    Success,
    Failed
}

public class RequestResponse
{
    public RequestResultEnum Result { get; init; }
    // public RequestEntityDTO? Request { get; init; }
    public string? ErrorMessage { get; init; }

    public static RequestResponse Success(
        // RequestEntityDTO Request
        ) =>
        new()
        {
            Result = RequestResultEnum.Success //,
            // Request = Request
        };

    public static RequestResponse Failed(RequestResultEnum result = RequestResultEnum.Failed, string? reason = null) =>
        new()
        {
            Result = result,
            ErrorMessage = reason ?? result switch
            {
                // RequestResultEnum.RequestAlreadyExists => "Request already exists",
                // RequestResultEnum.RequestNotFound => "Request not found",
                // RequestResultEnum.Forbid => "Forbidden",
                _ => null
            }
        };
}

public class RequestService(IRequestRepository repository)
{
    private readonly IRequestRepository _repository = repository;

    internal Task<RequestResponse> MethodExample()
    {
        throw new NotImplementedException();
    }
}
