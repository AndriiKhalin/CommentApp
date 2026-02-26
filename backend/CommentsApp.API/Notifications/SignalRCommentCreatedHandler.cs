using CommentsApp.API.Hubs;
using CommentsApp.Application.CQRS.Comments.Notifications;
using CommentsApp.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CommentsApp.API.Notifications;

public class SignalRCommentCreatedHandler(
    IBackgroundTaskQueue queue
) : INotificationHandler<CommentCreatedNotification>
{
    public async Task Handle(
        CommentCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        var comment = notification.Comment;

        await queue.EnqueueAsync(async (services, ct) =>
        {
            var hub = services.GetRequiredService<IHubContext<CommentsHub>>();
            await hub.Clients.All.SendAsync("NewComment", comment, ct);
        });
    }
}