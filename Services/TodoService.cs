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

        public async Task<List<Todo>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Todos
                .ToListAsync(cancellationToken);
        }

        public async Task<Todo?> GetByIdAsync(
            int id, 
            CancellationToken cancellationToken)
        {
            return await _context.Todos.FindAsync(
                new object[] { id },
                cancellationToken);
        }

        public async Task<Todo> CreateAsync(
            CreateTodoDto dto, 
            CancellationToken cancellationToken)
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
            await _context.SaveChangesAsync(cancellationToken);
            return todo;
        }

        public async Task<Todo?> UpdatedAsync(
            int id, 
            UpdateTodoDto dto, 
            CancellationToken cancellationToken)
        {
            var todo = await _context.Todos.FindAsync(
                new object[] { id },
                cancellationToken);
            if (todo == null)
                return null;

            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.IsCompleted = dto.IsCompleted;
            todo.Deadline = dto.Deadline;
            await _context.SaveChangesAsync(cancellationToken);

            return todo;
        }

        public async Task<bool> DeleteAsync(
            int id, 
            CancellationToken cancellationToken)
        {
            var todo = await _context.Todos.FindAsync(
                new object[] { id },
                cancellationToken);
            if (todo == null)
                return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<Todo>> GetIncompleteTodoAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Todos
                .Where(todo => !todo.IsCompleted)
                .OrderByDescending(todo => todo.CreatedAt)
                .Take(10)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> PatchAsync(
            int id, 
            PatchTodoDto dto, 
            CancellationToken cancellationToken)
        {
            var todo = await _context.Todos.FindAsync(
                new object[] { id }, 
                cancellationToken);
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

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
