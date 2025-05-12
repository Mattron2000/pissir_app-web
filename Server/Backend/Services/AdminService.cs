using Backend.Repositories;

namespace Backend.Services;

public enum AdminResultEnum
{
    Success,
    Failed
}

public class AdminResponse
{
    public AdminResultEnum Result { get; init; }
    // public AdminEntityDTO? Admin { get; init; }
    public string? ErrorMessage { get; init; }

    public static AdminResponse Success(
        // AdminEntityDTO Admin
        ) =>
        new()
        {
            Result = AdminResultEnum.Success //,
            // Admin = Admin
        };

    public static AdminResponse Failed(AdminResultEnum result = AdminResultEnum.Failed, string? reason = null) =>
        new()
        {
            Result = result,
            ErrorMessage = reason ?? result switch
            {
                // AdminResultEnum.AdminAlreadyExists => "Admin already exists",
                // AdminResultEnum.AdminNotFound => "Admin not found",
                // AdminResultEnum.Forbid => "Forbidden",
                _ => null
            }
        };
}

public class AdminService(IAdminRepository repository)
{
    private readonly IAdminRepository _repository = repository;

    internal Task<AdminResponse> MethodExample()
    {
        throw new NotImplementedException();
    }
}
