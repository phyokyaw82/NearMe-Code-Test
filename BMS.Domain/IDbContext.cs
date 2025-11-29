using BMS.Domain.BusinessLogic;
using BMS.Domain.Repository;

namespace BMS.Domain;

public interface IDbContext
{
    IBookLogic Book { get; }
}

