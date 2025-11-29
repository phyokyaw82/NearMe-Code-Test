using BMS.Domain.Repository;

namespace BMS.Domain;

public class DataContext : IDbContext
{
    public DataContext(IBookLogic book)
    {
        Book = book;
    }

    public IBookLogic Book { get; }
}

