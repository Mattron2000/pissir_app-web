using Backend.Repositories.Interfaces;

namespace Backend.Services;

public enum FineResultEnum
{
    Success,
    Failed
}

public class FineResponse
{
    public FineResultEnum Result { get; init; }
    // public FineEntityDTO? Fine { get; init; }
    public string? ErrorMessage { get; init; }

    public static FineResponse Success(
        // FineEntityDTO Fine
        ) =>
        new()
        {
            Result = FineResultEnum.Success //,
            // Fine = Fine
        };

    public static FineResponse Failed(FineResultEnum result = FineResultEnum.Failed, string? reason = null) =>
        new()
        {
            Result = result,
            ErrorMessage = reason ?? result switch
            {
                // FineResultEnum.FineAlreadyExists => "Fine already exists",
                // FineResultEnum.FineNotFound => "Fine not found",
                // FineResultEnum.Forbid => "Forbidden",
                _ => null
            }
        };
}

public class FineService(IFineRepository repository)
{
    private readonly IFineRepository _repository = repository;

    internal Task<FineResponse> MethodExample()
    {
        throw new NotImplementedException();
    }
}
