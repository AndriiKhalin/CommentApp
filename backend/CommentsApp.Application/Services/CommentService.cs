using CommentsApp.Application.DTOs;
using CommentsApp.Application.DTOs.Comments;
using CommentsApp.Application.Interfaces;
using CommentsApp.Domain.Entities;
using CommentsApp.Domain.Enums;
using CommentsApp.Domain.Interfaces;
using Ganss.Xss;

namespace CommentsApp.Application.Services;

public class CommentService(ICommentRepository repo) : ICommentService
{
    private readonly HtmlSanitizer _sanitizer = CreateSanitizer();

    // Дозволені теги: <a href="" title="">, <code>, <i>, <strong>
    private static HtmlSanitizer CreateSanitizer()
    {
        var s = new HtmlSanitizer();
        s.AllowedTags.Clear();
        s.AllowedTags.Add("a");
        s.AllowedTags.Add("code");
        s.AllowedTags.Add("i");
        s.AllowedTags.Add("strong");
        s.AllowedAttributes.Clear();
        s.AllowedAttributes.Add("href");
        s.AllowedAttributes.Add("title");
        s.AllowedSchemes.Add("http");
        s.AllowedSchemes.Add("https");
        return s;
    }

    public async Task<PagedResult<CommentDto>> GetCommentsAsync(
        int page, int pageSize, string sortBy, bool descending)
    {
        var (items, total) = await repo.GetRootCommentsAsync(page, pageSize, sortBy, descending);
        return new PagedResult<CommentDto>(
            items.Select(MapToDto),
            total, page, pageSize
        );
    }

    public async Task<CommentDto> CreateCommentAsync(
        CreateCommentRequest request, string? attachmentPath, string? attachmentType)
    {
        // Sanitize HTML — XSS protection
        var sanitizedText = _sanitizer.Sanitize(request.Text);

        var comment = new Comment
        {
            UserName = request.UserName,
            Email = request.Email,
            HomePage = request.HomePage,
            Text = sanitizedText,
            ParentId = request.ParentId,
            AttachmentPath = attachmentPath,
            AttachmentType = attachmentType != null
                ? Enum.Parse<AttachmentType>(attachmentType)
                : null,
            CreatedAt = DateTime.UtcNow
        };

        var created = await repo.AddAsync(comment);
        return MapToDto(created);
    }

    private CommentDto MapToDto(Comment c) => new(
        c.Id, c.UserName, c.Email, c.HomePage, c.Text,
        c.AttachmentPath, c.AttachmentType?.ToString(),
        c.CreatedAt, c.ParentId,
        c.Replies.Select(MapToDto).ToList()
    );
}