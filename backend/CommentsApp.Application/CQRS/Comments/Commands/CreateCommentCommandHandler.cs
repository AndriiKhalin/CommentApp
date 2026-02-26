using CommentsApp.Application.CQRS.Comments.Notifications;
using CommentsApp.Application.DTOs.Comments;
using CommentsApp.Application.Interfaces;
using MediatR;

namespace CommentsApp.Application.CQRS.Comments.Commands;

public class CreateCommentCommandHandler(
    ICommentService service,
    IPublisher publisher
) : IRequestHandler<CreateCommentCommand, CommentDto>
{
    public async Task<CommentDto> Handle(
        CreateCommentCommand request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateCommentAsync(
            request.Request,
            request.AttachmentPath,
            request.AttachmentType);

        await publisher.Publish(new CommentCreatedNotification(created), cancellationToken);
        return created;
    }
}