using BMS.Domain.Entity;
using FluentValidation;

namespace BMS.Api.Validators;

public class BookValidator : AbstractValidator<BookEntity>
{
    public BookValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required");

        RuleFor(x => x.Author)
            .NotEmpty()
            .WithMessage("Author is required");

        RuleFor(x => x.PublishedDate)
            .NotEmpty()
            .WithMessage("Published date is required");
    }
}

