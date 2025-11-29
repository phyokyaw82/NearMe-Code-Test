using BMS.Api.Controllers;
using BMS.Api.Test.Fakes;
using BMS.Api.Validators;
using BMS.Domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace BMS.Api.Tests;

public class BookControllerTests
{
    private readonly BookController _controller;

    public BookControllerTests()
    {
        var fakeBookLogic = new FakeBookLogic();
        var fakeDbContext = new FakeDbContext(fakeBookLogic);

        var bookValidator = new BookValidator();
        var idValidator = new BookIdValidator(fakeDbContext);

        _controller = new BookController(fakeDbContext, bookValidator, idValidator);
    }

    [Fact]
    public async Task Create_ReturnsCreatedBook()
    {
        var book = new BookEntity
        {
            Title = "Test Book",
            Author = "Author A",
            PublishedDate = DateTime.UtcNow
        };

        var result = await _controller.Create(book);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var createdBook = Assert.IsType<BookEntity>(okResult.Value);

        Assert.Equal("Test Book", createdBook.Title);
        Assert.NotEqual(Guid.Empty, createdBook.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsBooks()
    {
        var result = await _controller.GetAll();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var books = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<BookEntity>>(okResult.Value);

        Assert.NotNull(books);
    }

    [Fact]
    public async Task GetById_ReturnsBook()
    {
        var book = new BookEntity { Title = "Book 1", Author = "A", PublishedDate = DateTime.UtcNow };
        var created = await _controller.Create(book);
        var createdBook = ((OkObjectResult)created.Result).Value as BookEntity;

        var result = await _controller.GetById(createdBook.Id.ToString());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var foundBook = Assert.IsType<BookEntity>(okResult.Value);
        Assert.Equal(createdBook.Id, foundBook.Id);
    }

    [Fact]
    public async Task Update_ChangesBook()
    {
        var book = new BookEntity { Title = "Old Title", Author = "A", PublishedDate = DateTime.UtcNow };
        var created = await _controller.Create(book);
        var createdBook = ((OkObjectResult)created.Result).Value as BookEntity;

        var updated = new BookEntity { Title = "New Title", Author = "A", PublishedDate = DateTime.UtcNow };

        await _controller.Update(createdBook.Id.ToString(), updated);

        var result = await _controller.GetById(createdBook.Id.ToString());
        var okResult = Assert.IsType<OkObjectResult>(result);
        var updatedBook = Assert.IsType<BookEntity>(okResult.Value);

        Assert.Equal("New Title", updatedBook.Title);
    }

    [Fact]
    public async Task Delete_RemovesBook()
    {
        var book = new BookEntity { Title = "Book to Delete", Author = "A", PublishedDate = DateTime.UtcNow };
        var created = await _controller.Create(book);
        var createdBook = ((OkObjectResult)created.Result).Value as BookEntity;

        await _controller.Delete(createdBook.Id.ToString());

        var result = await _controller.GetAll();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var books = Assert.IsAssignableFrom<IEnumerable<BookEntity>>(okResult.Value);

        Assert.DoesNotContain(books, b => b.Id == createdBook.Id);
    }
}
