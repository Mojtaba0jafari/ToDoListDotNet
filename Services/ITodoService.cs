using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public interface ITodoService
    {
        Task<PagedResultDto<TodoResponseDto>> GetAllAsync(
        int userId, 
        int page,
        int pageSize,
        bool? isCompleted,
        string sortBy,
        string sortOrder,
        CancellationToken cancellationToken);
        Task<TodoResponseDto?> GetByIdAsync(
            int userId, 
            int id, 
            CancellationToken cancellationToken);
        Task<TodoResponseDto> CreateAsync(
            int userId, 
            CreateTodoDto dto, 
            CancellationToken cancellationToken);
        Task<TodoResponseDto?> UpdateAsync(
            int userId, 
            int id, 
            UpdateTodoDto dto, 
            CancellationToken cancellationToken);
        Task<bool> DeleteAsync(
            int userId, 
            int id, 
            CancellationToken cancellationToken);
        Task<TodoResponseDto?> PatchAsync(
            int userId, 
            int id, 
            PatchTodoDto dto, 
            CancellationToken cancellationToken);
    }
}
