namespace CommentsApp.Application.DTOs.Comments;

public record CreateCommentRequest(
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    string Captcha,
    int? ParentId
);