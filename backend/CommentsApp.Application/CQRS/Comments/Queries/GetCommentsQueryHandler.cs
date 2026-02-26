using CommentsApp.Application.DTOs;
using CommentsApp.Application.DTOs.Comments;
using CommentsApp.Application.Interfaces;
using MediatR;

namespace CommentsApp.Application.CQRS.Comments.Queries;

public sealed class GetCommentsQueryHandler(
    ICommentService service,
    ICacheService cache
) : IRequestHandler<GetCommentsQuery, PagedResult<CommentDto>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    public async Task<PagedResult<CommentDto>> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"comments:{request.Page}:{request.PageSize}:{request.SortBy}:{request.Descending}";

        var cached = await cache.GetAsync<PagedResult<CommentDto>>(key);
        if (cached is not null)
            return cached;

        var result = await service.GetCommentsAsync(
            request.Page,
            request.PageSize,
            request.SortBy,
            request.Descending);

        await cache.SetAsync(key, result, CacheDuration);
        return result;
    }
}