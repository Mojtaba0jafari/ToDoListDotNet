using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(
            RegisterRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(
                request,
                cancellationToken);
            // note to self: change here later
            if (result == null)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Email already registered",
                    Status = StatusCodes.Status409Conflict
                });
            }
                return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}
