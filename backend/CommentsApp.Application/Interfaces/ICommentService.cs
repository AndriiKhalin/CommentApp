using CommentsApp.Application.DTOs;
using CommentsApp.Application.DTOs.Comments;

namespace CommentsApp.Application.Interfaces;

public interface ICommentService
{
    Task<PagedResult<CommentDto>> GetCommentsAsync(
        int page, int pageSize, string sortBy, bool descending);

    Task<CommentDto> CreateCommentAsync(
        CreateCommentRequest request, string? attachmentPath, string? attachmentType);
}