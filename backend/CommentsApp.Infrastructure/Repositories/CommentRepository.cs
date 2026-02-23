using CommentsApp.Domain.Entities;
using CommentsApp.Domain.Interfaces;
using CommentsApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.Infrastructure.Repositories;

public class CommentRepository(CommentsDbContext context) : ICommentRepository
{
    public async Task<(IEnumerable<Comment> Items, int Total)> GetRootCommentsAsync(int page, int pageSize, string sortBy, bool descending)
    {
        IQueryable<Comment> query = context.Comments
            .Where(c => c.ParentId == null)
            .Include(c => c.Replies)
            .ThenInclude(r => r.Replies);

        query = sortBy switch
        {
            "userName" => descending
                ? query.OrderByDescending(c => c.UserName)
                : query.OrderBy(c => c.UserName),
            "email" => descending
                ? query.OrderByDescending(c => c.Email)
                : query.OrderBy(c => c.Email),
            _ => descending // default: LIFO by date
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Comment?> GetByIdWithRepliesAsync(int id)
    {
        return await context.Comments
            .Include(c => c.Replies)
            .ThenInclude(r => r.Replies)
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