using BMS.Domain;
using BMS.Domain.BusinessLogic;
using BMS.Domain.Repository;

namespace BMS.Api.Test.Fakes
{
    public class FakeDbContext : IDbContext
    {
        public IBookLogic Book { get; }

        public FakeDbContext(IBookLogic bookLogic)
        {
            Book = bookLogic;
        }
    }
}
