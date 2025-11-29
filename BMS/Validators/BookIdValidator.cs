using BMS.Domain;
using FluentValidation;

namespace BMS.Api.Validators;

public class BookIdValidator : AbstractValidator<string>
{
    public BookIdValidator(IDbContext db)
    {
        RuleFor(id => id)
            .NotEmpty().WithMessage("Book ID is required.")
            .MustAsync(async (id, cancellation) =>
            {
                var existing = await db.Book.FindByIdAsync(id, cancellation);
                return existing != null;
            })
            .WithMessage(id => $"Book with ID '{id}' does not exist.");
    }
}
