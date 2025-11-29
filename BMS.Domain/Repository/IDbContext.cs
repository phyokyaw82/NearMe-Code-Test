namespace BMS.Domain.Repository;

public interface IDbContext
{
    IBookLogic Book { get; }
}

