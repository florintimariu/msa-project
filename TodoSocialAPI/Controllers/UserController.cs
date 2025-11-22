using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoSocialApp.Data;
using TodoSocialApp.DTOs;
using TodoSocialApp.Models;
using BCrypt.Net;

namespace TodoSocialApp.Controllers;

[ApiController]
[Route("api")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(AppDbContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("/api/user")]
    public async Task<ActionResult<UserResponse>> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Email) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username, email, and password are required." });
            }

            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Conflict(new { message = "User with this email already exists." });
            }

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return Conflict(new { message = "Username is already taken." });
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { userId = user.UserId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return StatusCode(500, new { message = "An error occurred while creating the account." });
        }
    }

    [HttpGet("users/{userId}")]
    public async Task<ActionResult<UserResponse>> GetUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }

        var response = new UserResponse
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new UserResponse
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }
}
