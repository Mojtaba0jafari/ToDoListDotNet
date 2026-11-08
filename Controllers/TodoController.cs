using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTodos(
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] bool? isCompleted = null,
            [FromQuery, RegularExpression(
            "(?i)^(title|createdat|deadline)$")]
            string sortBy = "createdAt",
            [FromQuery, RegularExpression(
            "(?i)^(asc|desc)$")]
            string sortOrder = "desc",
            CancellationToken cancellationToken = default)
        {
            var todos = await _todoService.GetAllAsync(
                page,
                pageSize,
                isCompleted,
                sortBy,
                sortOrder,
                cancellationToken);
            
            return Ok(todos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
