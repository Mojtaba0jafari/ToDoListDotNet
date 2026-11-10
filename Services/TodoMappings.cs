using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    internal static class TodoMappings
    {
        public static TodoResponseDto ToResponse(
            this Todo todo,
            DateTime utcNow) => new()
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                Deadline = todo.Deadline,

                IsOverdue = !todo.IsCompleted
                        && todo.Deadline.HasValue
                        && todo.Deadline.Value < utcNow
            };
    }
}
