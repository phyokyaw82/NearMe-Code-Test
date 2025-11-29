using BMS.Api.Controllers;
using BMS.Api.Validators;
using BMS.Domain;
using BMS.Domain.Entity;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace BMS.Api.Tests.Controllers;

    public class BookControllerTests
{
    private readonly Mock<IDbContext> _dbMock;
    private readonly Mock<BookValidator> _bookValidatorMock;
    private readonly Mock<BookIdValidator> _bookIdValidatorMock;
    private readonly BookController _controller;

    public BookControllerTests()
    {
        _dbMock = new Mock<IDbContext>();

        // Mock BookValidator to always pass
        _bookValidatorMock = new Mock<BookValidator>();
        _bookValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<BookEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // success

        // Mock BookIdValidator to always pass
        _bookIdValidatorMock = new Mock<BookIdValidator>(_dbMock.Object);
        _bookIdValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // success

        _controller = new BookController(
            _dbMock.Object,
            _bookValidatorMock.Object,
            _bookIdValidatorMock.Object
        );
    }

    // GET /api/books
    [Fact]
    public async Task GetAll_ShouldReturnOkResult()
    {
        _dbMock.Setup(db => db.Book.FindAsync(It.IsAny<Expression<Func<BookEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookEntity>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<IEnumerable<BookEntity>>(okResult.Value);
    }

    // GET /api/books/{id}
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenBookExists()
    {
        var id = Guid.NewGuid();
        var book = new BookEntity { Id = id, Title = "Test", Author = "Author", PublishedDate = DateTime.Today };

        _dbMock.Setup(db => db.Book.FindByIdAsync(id.ToString(), default))
               .ReturnsAsync(book);

        var result = await _controller.GetById(id.ToString());

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(book, okResult.Value);
    }

    [Fact]
    public async Task GetById_ShouldThrowValidationException_WhenIdEmpty()
    {
        _bookIdValidatorMock
            .Setup(v => v.ValidateAsync("", default))
            .ThrowsAsync(new ValidationException("ID required"));

        await Assert.ThrowsAsync<ValidationException>(() => _controller.GetById(""));
    }

    // POST /api/books
    [Fact]
    public async Task Create_ShouldReturnCreatedResult_WhenValid()
    {
        var book = new BookEntity
        {
            Title = "Test Book",
            Author = "Author",
            PublishedDate = DateTime.Today
        };

        _dbMock.Setup(db => db.Book.InsertAsync(It.IsAny<BookEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookEntity b, CancellationToken _) => b); // return the same book

        var result = await _controller.Create(book);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(book, createdResult.Value);
    }

    [Fact]
    public async Task Create_ShouldThrowValidationException_WhenInvalid()
    {
        var book = new BookEntity { Title = "", Author = "", PublishedDate = DateTime.Today };

        _bookValidatorMock
            .Setup(v => v.ValidateAsync(book, default))
            .ThrowsAsync(new ValidationException("Invalid book"));

        await Assert.ThrowsAsync<ValidationException>(() => _controller.Create(book));
    }

    // PUT /api/books/{id}
    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenValid()
    {
        var id = Guid.NewGuid();
        var book = new BookEntity { Title = "Updated", Author = "Author", PublishedDate = DateTime.Today };

        _dbMock.Setup(db => db.Book.FindByIdAsync(id.ToString(), default))
               .ReturnsAsync(new BookEntity { Id = id });

        _dbMock.Setup(db => db.Book.UpdateByIdAsync(id.ToString(), It.IsAny<BookEntity>(), default))
               .Returns(Task.CompletedTask);

        var result = await _controller.Update(id.ToString(), book);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldThrowValidationException_WhenBookInvalid()
    {
        var id = Guid.NewGuid().ToString();
        var book = new BookEntity { Title = "", Author = "", PublishedDate = DateTime.Today };

        _bookValidatorMock
            .Setup(v => v.ValidateAsync(book, default))
            .ThrowsAsync(new ValidationException("Invalid book"));

        await Assert.ThrowsAsync<ValidationException>(() => _controller.Update(id, book));
    }

    // DELETE /api/books/{id}
    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenValid()
    {
        var id = Guid.NewGuid().ToString();

        _dbMock.Setup(db => db.Book.DeleteByIdAsync(id, default))
               .Returns(Task.CompletedTask);

        var result = await _controller.Delete(id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldThrowValidationException_WhenIdEmpty()
    {
        _bookIdValidatorMock
            .Setup(v => v.ValidateAsync("", default))
            .ThrowsAsync(new ValidationException("ID required"));

        await Assert.ThrowsAsync<ValidationException>(() => _controller.Delete(""));
    }
}
