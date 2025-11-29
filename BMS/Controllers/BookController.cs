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
    private readonly BookValidator _validator;

    public BookController(
        IDbContext db,
        BookValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    // GET /api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookEntity>>> GetAll(CancellationToken token = default)
    {
        var result = await _db.Book.FindAsync(token: token);
        return Ok(result);
    }

    // GET /api/books/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BookEntity>> GetById(string id, CancellationToken token = default)
    {
        var result = await _db.Book.FindByIdAsync(id: id, token: token);

        if (result == null)
            return NotFound($"Book with ID '{id}' not found.");

        return Ok(result);
    }

    // POST /api/books
    [HttpPost]
    public async Task<ActionResult<BookEntity>> Create(BookEntity data, CancellationToken token = default)
    {
        await _validator.ValidateAsync(data);

        var result = await _db.Book.InsertAsync(entity: data, token: token);

        // Return 201 Created with route to GET/{id}
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT /api/books/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, BookEntity data, CancellationToken token = default)
    {
        await _validator.ValidateAsync(data);

        var existing = await _db.Book.FindByIdAsync(id, token);
        if (existing == null)
            return NotFound($"Book with ID '{id}' not found.");

        await _db.Book.UpdateByIdAsync(id: id, entity: data, token: token);

        return NoContent(); // 204
    }

    // DELETE /api/books/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id, CancellationToken token = default)
    {
        var existing = await _db.Book.FindByIdAsync(id, token);
        if (existing == null)
            return NotFound($"Book with ID '{id}' not found.");

        await _db.Book.DeleteByIdAsync(id: id, token: token);

        return NoContent(); // 204
    }
}
