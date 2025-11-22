using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoSocialApp.Data;
using TodoSocialApp.DTOs;
using TodoSocialApp.Models;

namespace TodoSocialApp.Controllers;

[ApiController]
[Route("api")]
public class TodoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<TodoController> _logger;

    public TodoController(AppDbContext context, ILogger<TodoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("/api/todo")]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> GetTodos([FromQuery] int userId)
    {
        try
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Valid userId is required." });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var todos = await _context.Todos
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TodoResponse
                {
                    TodoId = t.TodoId,
                    UserId = t.UserId,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt,
                    CompletedAt = t.CompletedAt,
                    DueDate = t.DueDate
                })
                .ToListAsync();

            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todos");
            return StatusCode(500, new { message = "An error occurred while retrieving todos." });
        }
    }

    [HttpPost("/api/todo/")]
    public async Task<ActionResult<TodoResponse>> CreateTodo([FromQuery] int userId, [FromBody] CreateTodoRequest request)
    {
        try
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Valid userId is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "Title is required." });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var todo = new Todo
            {
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            var response = new TodoResponse
            {
                TodoId = todo.TodoId,
                UserId = todo.UserId,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                CompletedAt = todo.CompletedAt,
                DueDate = todo.DueDate
            };

            return CreatedAtAction(nameof(GetTodoById), new { todoId = todo.TodoId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo");
            return StatusCode(500, new { message = "An error occurred while creating the todo." });
        }
    }

    [HttpPut("/api/todo")]
    public async Task<ActionResult<TodoResponse>> UpdateTodo([FromQuery] int todoId, [FromBody] UpdateTodoRequest request)
    {
        try
        {
            if (todoId <= 0)
            {
                return BadRequest(new { message = "Valid todoId is required." });
            }

            var todo = await _context.Todos.FindAsync(todoId);
            if (todo == null)
            {
                return NotFound(new { message = "Todo not found." });
            }

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                todo.Title = request.Title;
            }

            if (request.Description != null)
            {
                todo.Description = request.Description;
            }

            if (request.IsCompleted.HasValue)
            {
                todo.IsCompleted = request.IsCompleted.Value;
                if (request.IsCompleted.Value && !todo.CompletedAt.HasValue)
                {
                    todo.CompletedAt = DateTime.UtcNow;
                }
                else if (!request.IsCompleted.Value)
                {
                    todo.CompletedAt = null;
                }
            }

            if (request.DueDate.HasValue)
            {
                todo.DueDate = request.DueDate.Value;
            }

            await _context.SaveChangesAsync();

            var response = new TodoResponse
            {
                TodoId = todo.TodoId,
                UserId = todo.UserId,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                CompletedAt = todo.CompletedAt,
                DueDate = todo.DueDate
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo");
            return StatusCode(500, new { message = "An error occurred while updating the todo." });
        }
    }

    [HttpDelete("/api/todo")]
    public async Task<ActionResult> DeleteTodo([FromQuery] int todoId)
    {
        try
        {
            if (todoId <= 0)
            {
                return BadRequest(new { message = "Valid todoId is required." });
            }

            var todo = await _context.Todos.FindAsync(todoId);
            if (todo == null)
            {
                return NotFound(new { message = "Todo not found." });
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Todo deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo");
            return StatusCode(500, new { message = "An error occurred while deleting the todo." });
        }
    }

    [HttpGet("/api/todo/{todoId}")]
    public async Task<ActionResult<TodoResponse>> GetTodoById(int todoId)
    {
        var todo = await _context.Todos.FindAsync(todoId);

        if (todo == null)
        {
            return NotFound(new { message = "Todo not found." });
        }

        var response = new TodoResponse
        {
            TodoId = todo.TodoId,
            UserId = todo.UserId,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt,
            CompletedAt = todo.CompletedAt,
            DueDate = todo.DueDate
        };

        return Ok(response);
    }
}
