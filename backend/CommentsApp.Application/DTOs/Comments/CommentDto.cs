namespace CommentsApp.Application.DTOs.Comments;

public record CommentDto(
    int Id,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    string? AttachmentPath,
    string? AttachmentType,
    DateTime CreatedAt,
    int? ParentId,
    List<CommentDto> Replies
);