using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status500InternalServerError,
        "application/problem+json")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        [HttpGet]
        [ProducesResponseType<PagedResultDto<TodoResponseDto>>(
            StatusCodes.Status200OK,
            "application/json")]
        [ProducesResponseType<ValidationProblemDetails>(
            StatusCodes.Status400BadRequest,
            "application/problem+json")]
        public async Task<ActionResult<PagedResultDto<TodoResponseDto>>> GetTodos(
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
        [ProducesResponseType<TodoResponseDto>(
            StatusCodes.Status200OK,
            "application/json")]
        [ProducesResponseType<ProblemDetails>(
            StatusCodes.Status404NotFound,
            "application/problem+json")]
        public async Task<ActionResult<TodoResponseDto>> GetTodoById(
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
        [ProducesResponseType<TodoResponseDto>(
            StatusCodes.Status201Created,
            "application/json")]
        [ProducesResponseType<ValidationProblemDetails>(
            StatusCodes.Status400BadRequest,
            "application/problem+json")]
        public async Task<ActionResult<TodoResponseDto>> CreateTodo(
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
        [ProducesResponseType<TodoResponseDto>(
            StatusCodes.Status200OK,
            "application/json")]
        [ProducesResponseType<ValidationProblemDetails>(
            StatusCodes.Status400BadRequest,
            "application/problem+json")]
        [ProducesResponseType<ProblemDetails>(
            StatusCodes.Status404NotFound,
            "application/problem+json")]
        public async Task<ActionResult<TodoResponseDto>> UpdateTodo(
            int id, 
            UpdateTodoDto dto,
            CancellationToken cancellationToken)
        {
            var todo = await _todoService.UpdateAsync(
                id, 
                dto,
                cancellationToken);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(
            StatusCodes.Status404NotFound,
            "application/problem+json")]
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
        [ProducesResponseType<TodoResponseDto>(
            StatusCodes.Status200OK,
            "application/json")]
        [ProducesResponseType<ValidationProblemDetails>(
            StatusCodes.Status400BadRequest,
            "application/problem+json")]
        [ProducesResponseType<ProblemDetails>(
            StatusCodes.Status404NotFound,
            "application/problem+json")]
        public async Task<ActionResult<TodoResponseDto>> PatchTodo(
            int id, 
            PatchTodoDto dto,
            CancellationToken cancellationToken)
        {
            var todo = await _todoService.PatchAsync(
                id, 
                dto,
                cancellationToken);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }
    }
}
