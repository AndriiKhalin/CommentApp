using CommentsApp.Domain.Entities;

namespace CommentsApp.Domain.Interfaces;

public interface ICommentRepository
{
    Task<(IEnumerable<Comment> Items, int Total)> GetRootCommentsAsync(
        int page, int pageSize, string sortBy, bool descending);

    Task<Comment?> GetByIdWithRepliesAsync(int id);
    Task<Comment> AddAsync(Comment comment);
    Task<bool> ExistsAsync(int id);
}