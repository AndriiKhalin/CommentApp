using CommentsApp.Application.DTOs.Comments;
using MediatR;

namespace CommentsApp.Application.CQRS.Comments.Notifications;

public record CommentCreatedNotification(CommentDto Comment) : INotification;