using Backend.Models;
using Backend.Repositories;
using Shared.DTOs;

namespace Backend.Services;

public class UserService(IUserRepository repository)
{
    private IUserRepository _repository { get; init; } = repository;

    public async Task<UserDTO[]> GetAllUsersAsync()
    {
        User[] users = await _repository.GetAllUsersAsync();

        return [.. users.Select(user => new UserDTO(
            user.Email,
            user.Name,
            user.Surname,
            user.Type
        ))];
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email)
    {
        User? user = await _repository.GetUserByEmailAsync(email);

        if (user == null)
            return null;
        else
            return new UserDTO(
                user.Email,
                user.Name,
                user.Surname,
                user.Type
            );
    }
}
