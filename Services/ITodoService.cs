using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public interface ITodoService
    {
        Task<List<Todo>> GetAllAsync();
        Task<Todo?> GetByIdAsync(int id);
        Task<Todo> CreateAsync(CreateTodoDto dto);
        Task<Todo?> UpdatedAsync(int id, UpdateTodoDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<Todo>> GetIncompleteTodoAsync();
    }
}
