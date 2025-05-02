using Backend.Repositories;
using Shared.DTOs;

namespace Backend.Services;

public class UserService(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public async Task<UserDTO[]> GetAllUsersAsync()
    {
        var users = await _repository.GetAllUsersAsync();

        return [.. users.Select(user => new UserDTO(
            user.Email,
            user.Name,
            user.Surname,
            user.Type
        ))];
    }
}
