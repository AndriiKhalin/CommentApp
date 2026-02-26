using CommentsApp.Domain.Entities;
using CommentsApp.Domain.Interfaces;
using CommentsApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.Infrastructure.Repositories;

public class CommentRepository(CommentsDbContext context) : ICommentRepository
{
    public async Task<(IEnumerable<Comment> Items, int Total)> GetRootCommentsAsync(int page, int pageSize,
        string sortBy, bool descending)
    {
        var baseQuery = context.Comments
            .AsNoTracking()
            .Where(c => c.ParentId == null);

        var total = await baseQuery.CountAsync();

        var queryWithIncludes = baseQuery
            .Include(c => c.Replies)
            .ThenInclude(r => r.Replies)
            .AsSplitQuery();

        queryWithIncludes = sortBy switch
        {
            "userName" => descending
                ? queryWithIncludes.OrderByDescending(c => c.UserName)
                : queryWithIncludes.OrderBy(c => c.UserName),
            "email" => descending
                ? queryWithIncludes.OrderByDescending(c => c.Email)
                : queryWithIncludes.OrderBy(c => c.Email),
            _ => descending
                ? queryWithIncludes.OrderByDescending(c => c.CreatedAt)
                : queryWithIncludes.OrderBy(c => c.CreatedAt)
        };

        var items = await queryWithIncludes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Comment?> GetByIdWithRepliesAsync(int id)
    {
        return await context.Comments
            .AsNoTracking()
            .Include(c => c.Replies)
            .ThenInclude(r => r.Replies)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Comments.AnyAsync(c => c.Id == id);
    }
}