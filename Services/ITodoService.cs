using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public interface ITodoService
    {
        Task<List<Todo>> GetAllAsync(CancellationToken cancellationToken);
        Task<Todo?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Todo> CreateAsync(CreateTodoDto dto, CancellationToken cancellationToken);
        Task<Todo?> UpdatedAsync(int id, UpdateTodoDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<List<Todo>> GetIncompleteTodoAsync(CancellationToken cancellationToken);
        Task<bool> PatchAsync(int id, PatchTodoDto dto, CancellationToken cancellationToken);
    }
}
