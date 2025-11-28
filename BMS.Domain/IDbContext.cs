using BMS.Domain.BusinessLogic;

namespace BMS.Domain;

public interface IDbContext
{
    Book Book { get; }
}

