using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Domain;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTodos()
        {
            var todo = new List<Todo>
            {
                new Todo
                {
                    Id = 1,
                    Title = "Learn ASP.NET Core",
                    Description = "Build my first API",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    Deadline = DateTime.UtcNow,
                },
                new Todo
                {
                    Id = 2,
                    Title = "Build Todo API",
                    Description = "Create the first endpoint",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    Deadline = DateTime.UtcNow
                }
            };
            return Ok(todo);
        }
    }
}
