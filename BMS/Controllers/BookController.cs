using BMS.Api.Validators;
using BMS.Domain;
using BMS.Domain.Entity;
using Microsoft.AspNetCore.Mvc;

namespace BMS.Api.Controllers;

[ApiController]
public class BookController : ControllerBase
{
    private readonly IDbContext _db;
    private readonly BookValidator _validator;
    public BookController(IDbContext db,
        BookValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    [Route("GET/api/books"), HttpGet]
    public async Task<ActionResult<IEnumerable<BookEntity>>> Get(CancellationToken token = default)
    {
        try
        {
            var result = await _db.Book.FindAsync(token: token);
            return Ok(result);
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }
    }

    [Route("GET/api/books/{id}"), HttpGet]
    public async Task<ActionResult<BookEntity>> Get(string id,
        CancellationToken token = default)
    {
        try
        {
            var result = await _db.Book.FindAsync(filter: f => f.Id == Guid.Parse(id), token: token);
            return Ok(result);
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }
    }

    [Route("POST/api/books"), HttpPost]
    public async Task<ActionResult<BookEntity>> Post(BookEntity data,
        CancellationToken token = default)
    {
        try
        {
            await _validator.ValidateAsync(data);

            var result = await _db.Book.InsertAsync(entity: data, token: token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("PUT/api/books/{id}"), HttpPut]
    public async Task<ActionResult> Put(BookEntity data,
        CancellationToken token = default)
    {
        try
        {
            await _validator.ValidateAsync(data);

            await _db.Book.UpdateAsync(entity: data,
                filter: f => f.Id == data.Id,
                token: token);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("DELETE/api/books/{id}"), HttpDelete]
    public async Task<ActionResult> Delete(string id,
        CancellationToken token = default)
    {
        try
        {
            await _db.Book.DeleteAsync(filter: f => f.Id == Guid.Parse(id),
                token: token);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

