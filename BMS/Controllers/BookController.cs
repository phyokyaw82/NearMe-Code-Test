using BMS.Api.Validators;
using BMS.Domain;
using BMS.Domain.Entity;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BMS.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly IDbContext _db;
    private readonly BookValidator _validator;
    private readonly BookIdValidator _idValidator;

    public BookController(
        IDbContext db,
        BookValidator validator,
        BookIdValidator idValidator)
    {
        _db = db;
        _validator = validator;
        _idValidator = idValidator;
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
        await _validator.ValidateAsync(data);                 // validate payload
        await _idValidator.ValidateAndThrowAsync(id, token); // validate ID existence

        await _db.Book.UpdateByIdAsync(id, data, token);

        return NoContent();
    }


    // DELETE /api/books/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id, CancellationToken token = default)
    {
        await _idValidator.ValidateAndThrowAsync(id, token);
        await _db.Book.DeleteByIdAsync(id, token);

        return NoContent();
    }
}
