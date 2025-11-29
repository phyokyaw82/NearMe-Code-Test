using BMS.Core.Data;
using BMS.Domain.DAO;
using BMS.Domain.Entity;
using BMS.Domain.Repository;

namespace BMS.Domain.BusinessLogic;
public class Book : BusinessLogic<BookEntity, BookDAO>, IBookLogic
{
}

