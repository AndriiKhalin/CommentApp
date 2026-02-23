using CommentsApp.Domain.Enums;

namespace CommentsApp.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? HomePage { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public AttachmentType? AttachmentType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Self-referencing for nested comments
    public int? ParentId { get; set; }
    public Comment? Parent { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}