using CommentsApp.Application.DTOs.Comments;
using MediatR;

namespace CommentsApp.Application.CQRS.Comments.Commands;

public record CreateCommentCommand(
    CreateCommentRequest Request,
    string? AttachmentPath,
    string? AttachmentType
) : IRequest<CommentDto>;