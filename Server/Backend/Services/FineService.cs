using Backend.Models;
using Backend.Repositories.Interfaces;
using Shared.DTOs.Fine;

namespace Backend.Services;

public enum FineResultEnum
{
    Success,
    Failed
}

public class FineResponse
{
    public FineResultEnum Result { get; init; }
    public string? ErrorMessage { get; init; }
    public FineEntityDTO[]? Fines { get; internal set; }

    public static FineResponse Success(FineEntityDTO[] value) =>
        new()
        {
            Result = FineResultEnum.Success,
            Fines = value
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

    internal async Task<FineResponse> AddFineAsync(FineNewDTO fineDTO)
    {
        if (fineDTO == null)
            return FineResponse.Failed(FineResultEnum.Failed, "Fine body is null");

        Fine? fine = await _repository.AddFineAsync(fineDTO);

        if (fine != null)
            return FineResponse.Success([new FineEntityDTO(fine.Email, fine.DatetimeStart.ToString(), fine.DatetimeEnd.ToString(), fine.Kw, fine.Paid == true)]);

        return FineResponse.Failed(FineResultEnum.Failed, "Fine not added");
    }

    internal async Task<FineResponse> GetFinesAsync()
    {
        Fine[]? fines = await _repository.GetFinesAsync();

        if (fines == null)
            return FineResponse.Failed(FineResultEnum.Failed, "Fines not found");

        return FineResponse.Success(
            [.. fines.Select(f => new FineEntityDTO(f.Email, f.DatetimeStart.ToString(), f.DatetimeEnd.ToString(), f.Kw, f.Paid == true))]
        );
    }

    internal async Task<FineResponse> GetUserFinesAsync(string email)
    {
        Fine[]? fines = await _repository.GetUserFinesAsync(email);

        if (fines == null)
            return FineResponse.Failed(FineResultEnum.Failed, "Fines not found");

        return FineResponse.Success(
            [.. fines.Select(f => new FineEntityDTO(f.Email, f.DatetimeStart.ToString(), f.DatetimeEnd.ToString(), f.Kw, f.Paid == true))]
        );
    }

    internal async Task<FineResponse> UpdateFineAsync(string email, string datetime)
    {
        Fine? fine = await _repository.GetUserFineAsync(email, datetime);

        if (fine == null)
            return FineResponse.Failed(FineResultEnum.Failed, "Fine not found");

        fine = await _repository.UpdateFineAsync(email, datetime);

        if (fine == null)
            return FineResponse.Failed(FineResultEnum.Failed, "Fine not updated");

        return FineResponse.Success([
            new FineEntityDTO(
                fine.Email,
                fine.DatetimeStart.ToString(),
                fine.DatetimeEnd.ToString(),
                fine.Kw,
                fine.Paid == true)
        ]);
    }
}
