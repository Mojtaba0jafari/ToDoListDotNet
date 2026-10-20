using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Services;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var todo = await _todoService.GetAllAsync();
            
            return Ok(todo);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            var todo = await _todoService.GetByIdAsync(id);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoDto dto) 
        {
            var todo = await _todoService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetTodoById), 
                new {id = todo.Id}, 
                todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, UpdateTodoDto dto)
        {
            var todo = await _todoService.UpdatedAsync(id, dto);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var delete = await _todoService.DeleteAsync(id);
            if (!delete)
                return NotFound();

            return NoContent();
        }
    }
}
