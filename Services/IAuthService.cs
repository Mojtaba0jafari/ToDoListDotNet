using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public interface IAuthService
    {
        Task<RegisterResponseDto?> RegisterAsync(
            RegisterRequestDto request,
            CancellationToken cancellationToken);
    }
}
