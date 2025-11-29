using BMS.Api.Validators;
using BMS.Domain;
using BMS.Domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace BMS.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly IDbContext _db;
    private readonly BookValidator _bookValidator;
    private readonly BookIdValidator _idValidator;

    public BookController(IDbContext db,
        BookValidator bookValidator,
        BookIdValidator idValidator)
    {
        _db = db;
        _bookValidator = bookValidator;
        _idValidator = idValidator;
    }

    // GET /api/books
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _db.Book.FindAsync();
        return Ok(books);
    }

    // GET /api/books/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        await _idValidator.ValidateAsync(id);

        var book = await _db.Book.FindByIdAsync(id);
        return Ok(book);
    }

    // POST /api/books
    [HttpPost]
    public async Task<ActionResult<BookEntity>> Create([FromBody] BookEntity book)
    {
        await _bookValidator.ValidateAsync(book);
        var result = await _db.Book.InsertAsync(book);

        return Ok(result);
    }

    // PUT /api/books/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] BookEntity book)
    {
        // Validate ID exists
        await _idValidator.ValidateAsync(id);

        // Validate book entity
        await _bookValidator.ValidateAsync(book);
        await _db.Book.UpdateByIdAsync(id, book);

        return NoContent();
    }

    // DELETE /api/books/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _idValidator.ValidateAsync(id);
        await _db.Book.DeleteByIdAsync(id);

        return NoContent();
    }
}
