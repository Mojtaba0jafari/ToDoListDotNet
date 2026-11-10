using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public interface ITodoService
    {
        Task<PagedResultDto<TodoResponseDto>> GetAllAsync(
        int page,
        int pageSize,
        bool? isCompleted,
        string sortBy,
        string sortOrder,
        CancellationToken cancellationToken);
        Task<TodoResponseDto?> GetByIdAsync(
            int id, CancellationToken cancellationToken);
        Task<TodoResponseDto> CreateAsync(
            CreateTodoDto dto, CancellationToken cancellationToken);
        Task<TodoResponseDto?> UpdateAsync(
            int id, UpdateTodoDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<TodoResponseDto?> PatchAsync(
            int id, PatchTodoDto dto, CancellationToken cancellationToken);
    }
}
