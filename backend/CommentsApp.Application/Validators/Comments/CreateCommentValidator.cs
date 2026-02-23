using CommentsApp.Application.DTOs.Comments;
using FluentValidation;

namespace CommentsApp.Application.Validators.Comments;

public class CreateCommentValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required")
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9]+$")
            .WithMessage("UserName must contain only Latin letters and digits");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.HomePage)
            .Must(url => url == null || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("HomePage must be a valid URL");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text is required")
            .MaximumLength(5000);

        RuleFor(x => x.Captcha)
            .NotEmpty()
            .Matches(@"^[a-zA-Z0-9]+$");
    }
}