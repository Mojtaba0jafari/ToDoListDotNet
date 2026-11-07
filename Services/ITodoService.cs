using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public interface ITodoService
    {
        Task<PagedResultDto<Todo>> GetAllAsync(
        int page,
        int pageSize,
        bool? isCompleted,
        CancellationToken cancellationToken);
        Task<Todo?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Todo> CreateAsync(CreateTodoDto dto, CancellationToken cancellationToken);
        Task<Todo?> UpdatedAsync(int id, UpdateTodoDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> PatchAsync(int id, PatchTodoDto dto, CancellationToken cancellationToken);
    }
}
