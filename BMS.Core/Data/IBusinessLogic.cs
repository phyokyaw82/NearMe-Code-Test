using System.Linq.Expressions;

namespace BMS.Core.Data;

public interface IBusinessLogic<TEntity> where TEntity : EntityBase, new()
{
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken token = default);
    Task<TEntity?> FindByIdAsync(string id, CancellationToken token = default);
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken token = default);
    Task UpdateByIdAsync(string id, TEntity entity, CancellationToken token = default);
    Task DeleteByIdAsync(string id, CancellationToken token = default);
}

