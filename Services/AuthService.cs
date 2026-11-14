using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoList.Api.Data;
using ToDoList.Api.Domain;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly TodoDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        public AuthService(
            TodoDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
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

        public async Task<LoginResponseDto?> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _context.Users
                .FirstOrDefaultAsync(
                user => user.Email == email,
                cancellationToken );
            if (user == null) 
                return null;

            var passwordresult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);
            if (passwordresult == PasswordVerificationResult.Failed)
                return null;

            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is not configured.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT issuer is not configured.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT audience is not configured.");

            var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes");

            if (expirationMinutes <= 0)
                throw new InvalidOperationException("JWT expiration must be greater than zero.");

            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingKey = new SymmetricSecurityKey(
                Convert.FromBase64String(key));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAt
            };

        }
    }
}
