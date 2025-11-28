using BMS.Domain.BusinessLogic;

namespace BMS.Domain;

public class DataContext : IDbContext
{
    public DataContext(Book book)
    {
        Book = book;
    }

    public Book Book { get; }
}

