using BMS.Domain.BusinessLogic;

namespace BMS.Domain.Repository;

public interface IDbContext
{
    IBookLogic Book { get; }
}

