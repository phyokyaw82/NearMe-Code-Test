using BMS.Domain.Entity;
using FluentValidation;

namespace BMS.Api.Validators;

public class BookValidator : AbstractValidator<BookEntity>
{
    public BookValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Author)
            .NotEmpty()
            .NotEmpty().WithMessage("Author is required");

        RuleFor(x => x.PublishedDate)
            .NotEmpty()
            .WithMessage("Published date is required");
    }
}

