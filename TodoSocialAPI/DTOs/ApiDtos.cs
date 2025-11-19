namespace TodoSocialApp.DTOs;

// User DTOs
public class CreateAccountRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Todo DTOs
public class CreateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
}

public class UpdateTodoRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
}

public class TodoResponse
{
    public int TodoId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DueDate { get; set; }
}

// Friend DTOs
public class AddFollowRequest
{
    public int FollowedUserId { get; set; }
}

public class FriendResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FollowedAt { get; set; }
}

// Message DTOs
public class CreateMessageRequest
{
    public int ReceiverId { get; set; }
    public string MessageType { get; set; } = "text";
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
}

public class MessageResponse
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}
