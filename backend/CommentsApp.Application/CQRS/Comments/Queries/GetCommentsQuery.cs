using CommentsApp.Application.DTOs;
using CommentsApp.Application.DTOs.Comments;
using MediatR;

namespace CommentsApp.Application.CQRS.Comments.Queries;

public record GetCommentsQuery(
    int Page,
    int PageSize,
    string SortBy,
    bool Descending
) : IRequest<PagedResult<CommentDto>>;