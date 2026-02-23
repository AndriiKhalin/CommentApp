using Microsoft.AspNetCore.SignalR;

namespace CommentsApp.API.Hubs;

public static class CommentsHubExtensions
{
    public static async Task NotifyNewComment(
        this IHubContext<CommentsHub> hub, object comment)
    {
        await hub.Clients.All.SendAsync("NewComment", comment);
    }
}