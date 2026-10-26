using Microsoft.AspNetCore.Mvc;
using System.Threading;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services;

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
        public async Task<IActionResult> GetTodos(
            CancellationToken cancellationToken)
        {
            var todo = await _todoService.GetAllAsync(cancellationToken);
            
            return Ok(todo);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(
            int id,
            CancellationToken cancellationToken)
        {
            var todo = await _todoService.GetByIdAsync(
                id, 
                cancellationToken);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTodo(
            CreateTodoDto dto,
            CancellationToken cancellationToken) 
        {
            var todo = await _todoService.CreateAsync(
                dto,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetTodoById), 
                new {id = todo.Id}, 
                todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(
            int id, 
            UpdateTodoDto dto,
            CancellationToken cancellationToken)
        {
            var todo = await _todoService.UpdatedAsync(
                id, 
                dto,
                cancellationToken);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(
            int id,
            CancellationToken cancellationToken)
        {
            var delete = await _todoService.DeleteAsync(
                id,
                cancellationToken);
            if (!delete)
                return NotFound();

            return NoContent();
        }

        [HttpGet("incomplete")]
        public async Task<IActionResult> GetInComplete(
            CancellationToken cancellationToken)
        {
            var todo = await _todoService.GetIncompleteTodoAsync(
                cancellationToken);
            if (todo == null) 
                return NotFound();

            return Ok(todo);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(
            int id, 
            PatchTodoDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _todoService.PatchAsync(
                id, 
                dto,
                cancellationToken);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
