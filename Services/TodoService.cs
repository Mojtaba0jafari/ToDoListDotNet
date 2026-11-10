using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Data;
using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoDbContext _context;
        private readonly TimeProvider _timeProvider;
        public TodoService(TodoDbContext context, TimeProvider timeProvider)
        {
            _context = context;
            _timeProvider = timeProvider;
        }

        public async Task<PagedResultDto<TodoResponseDto>> GetAllAsync(
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
                (sortBy.ToLowerInvariant(), sortOrder.ToLowerInvariant()) switch
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
            var now = UtcNow();

            return new PagedResultDto<TodoResponseDto>
            {
                Items = todos.Select(todo => todo.ToResponse(now)).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };
        }

        public async Task<TodoResponseDto?> GetByIdAsync(
            int id, 
            CancellationToken cancellationToken)
        {
            var todo = await _context.Todos.FindAsync(
                new object[] { id },
                cancellationToken);

            return todo?.ToResponse(UtcNow());
        }

        public async Task<TodoResponseDto> CreateAsync(
            CreateTodoDto dto, 
            CancellationToken cancellationToken)
        {
            var now = UtcNow();
            var todo = new Todo
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = false,
                CreatedAt = now,
                Deadline = ToUtc(dto.Deadline)
            };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync(cancellationToken);
            return todo.ToResponse(now);
        }

        public async Task<TodoResponseDto?> UpdateAsync(
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
            todo.IsCompleted = dto.IsCompleted!.Value;
            todo.Deadline = ToUtc(dto.Deadline);
            await _context.SaveChangesAsync(cancellationToken);

            return todo.ToResponse(UtcNow());
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

        public async Task<TodoResponseDto?> PatchAsync(
            int id, 
            PatchTodoDto dto, 
            CancellationToken cancellationToken)
        {
            var todo = await _context.Todos.FindAsync(
                new object[] { id }, 
                cancellationToken);
            if (todo == null) 
                return null;

            if (dto.Title != null)
                todo.Title = dto.Title;
            if (dto.DescriptionSpecified)
                todo.Description = dto.Description;
            if (dto.IsCompleted.HasValue)
                todo.IsCompleted = dto.IsCompleted.Value;
            if (dto.DeadlineSpecified)
                todo.Deadline = ToUtc(dto.Deadline);

            await _context.SaveChangesAsync(cancellationToken);
            return todo.ToResponse(UtcNow());
        }

        private DateTime UtcNow() => _timeProvider.GetUtcNow().UtcDateTime;

        private static DateTime? ToUtc(DateTime? value) => value switch
        {
            null => null,
            { Kind: DateTimeKind.Utc } => value,
            { Kind: DateTimeKind.Local } => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }
}
