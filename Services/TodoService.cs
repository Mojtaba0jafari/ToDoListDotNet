using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Data;
using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoDbContext _context;
        public TodoService(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Todo>> GetAllAsync()
        {
            return await _context.Todos.ToListAsync();
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            return await _context.Todos.FindAsync(id);
        }

        public async Task<Todo> CreateAsync(CreateTodoDto dto)
        {
            var todo = new Todo
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                Deadline = dto.Deadline
            };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo?> UpdatedAsync(int id, UpdateTodoDto dto)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                return null;

            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.IsCompleted = dto.IsCompleted;
            todo.Deadline = dto.Deadline;
            await _context.SaveChangesAsync();

            return todo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Todo>> GetIncompleteTodoAsync()
        {
            return await _context.Todos
                .Where(todo => !todo.IsCompleted)
                .OrderByDescending(todo => todo.CreatedAt)
                .Take(10)
                .ToListAsync();
        }

        public async Task<bool> PatchAsync(int id, PatchTodoDto dto)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) 
                return false;

            if (dto.Title != null)
                todo.Title = dto.Title;
            if (dto.DescriptionSpecified)
                todo.Description = dto.Description;
            if (dto.IsCompleted.HasValue)
                todo.IsCompleted = dto.IsCompleted.Value;
            if (dto.Deadline.HasValue)
                todo.Deadline = dto.Deadline.Value;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
