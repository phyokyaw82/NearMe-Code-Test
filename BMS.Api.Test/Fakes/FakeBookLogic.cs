using BMS.Domain.Entity;
using BMS.Domain.Repository;
using System.Linq.Expressions;

namespace BMS.Api.Test.Fakes;

public class FakeBookLogic : IBookLogic
{
    private readonly List<BookEntity> _books = new();

    public Task<BookEntity> InsertAsync(BookEntity entity, 
        CancellationToken token = default)
    {
        entity.Id = Guid.NewGuid();
        _books.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<BookEntity>> FindAsync(Expression<Func<BookEntity, bool>> filter = null, CancellationToken token = default)
    {
        var query = _books.AsQueryable();
        if (filter != null)
            query = query.Where(filter);
        return Task.FromResult(query.AsEnumerable());
    }

    public Task<BookEntity?> FindByIdAsync(string id, CancellationToken token = default)
    {
        var book = _books.SingleOrDefault(b => b.Id.ToString() == id);
        return Task.FromResult(book);
    }

    public Task UpdateAsync(BookEntity entity, Expression<Func<BookEntity, bool>> filter, CancellationToken token = default)
    {
        var existing = _books.AsQueryable().Where(filter).FirstOrDefault();
        if (existing != null)
        {
            existing.Title = entity.Title;
            existing.Author = entity.Author;
            existing.PublishedDate = entity.PublishedDate;
        }
        return Task.CompletedTask;
    }

    public Task UpdateByIdAsync(string id, BookEntity entity, CancellationToken token = default)
    {
        var existing = _books.SingleOrDefault(b => b.Id.ToString() == id);
        if (existing != null)
        {
            existing.Title = entity.Title;
            existing.Author = entity.Author;
            existing.PublishedDate = entity.PublishedDate;
        }
        return Task.CompletedTask;
    }

    public Task DeleteByIdAsync(string id, CancellationToken token = default)
    {
        _books.RemoveAll(b => b.Id.ToString() == id);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Expression<Func<BookEntity, bool>> filter, CancellationToken token = default)
    {
        _books.RemoveAll(b => filter.Compile()(b));
        return Task.CompletedTask;
    }
}
