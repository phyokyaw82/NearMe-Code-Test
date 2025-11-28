using System.Linq.Expressions;

namespace BMS.Core.Data.QueryBuilder
{
    public interface IQueryBuilder<TEntity> where TEntity : EntityBase, new()
    {
        string BuildSelect(
            Expression<Func<TEntity, dynamic>> selector = null,
            Expression<Func<TEntity, bool>> filter = null);

        string BuildInsert(Expression<Func<TEntity, TEntity>> selector);

        string BuildUpdate(
            Expression<Func<TEntity, TEntity>> selector,
            Expression<Func<TEntity, bool>> filter);

        string BuildDelete(Expression<Func<TEntity, bool>> filter);
    }
}