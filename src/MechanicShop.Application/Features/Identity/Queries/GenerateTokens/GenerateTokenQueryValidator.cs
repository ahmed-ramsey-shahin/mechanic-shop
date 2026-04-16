using FluentValidation;

namespace MechanicShop.Application.Features.Identity.Queries.GenerateTokens;

public class GenerateTokenQueryValidator : AbstractValidator<GenerateTokenQuery>
{
    public GenerateTokenQueryValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().EmailAddress()
            .WithErrorCode("EmailNullOrEmpty")
            .WithMessage("Email can not be null or empty");
        RuleFor(request => request.Password)
            .NotEmpty()
            .WithErrorCode("PasswrordNullOrEmpty")
            .WithMessage("Password can not be null or empty");
    }
}
