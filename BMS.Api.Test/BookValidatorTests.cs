using BMS.Api.Validators;
using BMS.Domain;
using BMS.Domain.Entity;
using FluentValidation.TestHelper;

namespace BMS.Api.Tests.Validators;

public class BookValidatorTests
{
    private readonly BookValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var book = new BookEntity { Title = "", Author = "Author", PublishedDate = DateTime.Today };
        var result = _validator.TestValidate(book);
        result.ShouldHaveValidationErrorFor(b => b.Title);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Book_Is_Valid()
    {
        var book = new BookEntity
        {
            Title = "Valid Title",
            Author = "Author Name",
            PublishedDate = DateTime.Today,
            CategoryId = BookCategory.Other
        };
        var result = _validator.TestValidate(book);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
