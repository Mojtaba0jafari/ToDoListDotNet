using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;
using ToDoList.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext _context;

        public TodoController(TodoDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var todo = await _context.Todos.ToListAsync();
            
            return Ok(todo);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoDto dto) 
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

            return Ok(todo);
        }
        
    }
}
