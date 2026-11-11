using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Data;
using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly TodoDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthService(
            TodoDbContext context,
            IPasswordHasher<User> passwordHasher )
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterResponseDto?> RegisterAsync(
            RegisterRequestDto request,
            CancellationToken cancellationToken )
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var existingUser = await _context.Users
                .AnyAsync( user => user.Email == email,
                cancellationToken );
            if (existingUser)
                return null;

            var user = new User
            {
                Email = email,
                Name = request.Name.Trim()
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password);

            _context.Users.Add( user );
            await _context.SaveChangesAsync(cancellationToken);
            return new RegisterResponseDto
            { 
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            };
        }
    }
}
