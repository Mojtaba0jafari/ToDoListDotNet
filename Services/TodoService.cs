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

        public async Task<PagedResultDto<Todo>> GetAllAsync(
            int page,
            int pageSize,
            bool? isCompleted,
            string sortBy,
            string sortOrder,
            CancellationToken cancellationToken)
        {
            var query = _context.Todos.AsNoTracking();

            if (isCompleted.HasValue)
                query = query.Where(todo => todo.IsCompleted == isCompleted.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var orderedQuery = 
                (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("title", "asc") => query.OrderBy(todo => todo.Title),
                ("title", "desc") => query.OrderByDescending(todo => todo.Title),

                ("createdat", "asc") => query.OrderBy(todo => todo.CreatedAt),
                ("createdat", "desc") => query.OrderByDescending(todo => todo.CreatedAt),

                ("deadline", "asc") => query.OrderBy(todo => todo.Deadline),
                ("deadline", "desc") => query.OrderByDescending(todo => todo.Deadline),

                _ => query.OrderByDescending(todo => todo.CreatedAt)
            };

            var skip = (int)Math.Min(
                ((long)page - 1) * pageSize,
                int.MaxValue);

            var todos = await orderedQuery
                .ThenByDescending(todo => todo.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(
                totalCount/ (double)pageSize);

            return new PagedResultDto<Todo>
            {
                Items = todos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };
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
