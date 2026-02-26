using CommentsApp.Application.CQRS.Comments.Commands;
using CommentsApp.Domain.Interfaces;
using FluentValidation;

namespace CommentsApp.Application.Validators.Comments;

public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator(ICommentRepository repo)
    {
        RuleFor(x => x.Request.UserName)
            .NotEmpty().WithMessage("UserName is required")
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9]+$")
            .WithMessage("UserName must contain only Latin letters and digits");

        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Request.HomePage)
            .Must(url => url == null || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("HomePage must be a valid URL");

        RuleFor(x => x.Request.Text)
            .NotEmpty().WithMessage("Text is required")
            .MaximumLength(5000);

        RuleFor(x => x.Request.Captcha)
            .NotEmpty()
            .Matches(@"^[a-zA-Z0-9]+$");

        RuleFor(x => x.Request.ParentId)
            .MustAsync(async (parentId, ct) =>
                parentId == null || await repo.ExistsAsync(parentId.Value))
            .WithMessage("Parent comment does not exist");
    }
}