using Microsoft.AspNetCore.SignalR;

namespace CommentsApp.API.Hubs;

public class CommentsHub : Hub
{
    public async Task JoinCommentThread(string commentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"comment_{commentId}");
    }
}