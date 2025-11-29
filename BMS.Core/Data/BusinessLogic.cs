using System.Linq.Expressions;

namespace BMS.Core.Data;

public abstract class BusinessLogic<TEntity, TDAO>
      where TEntity : EntityBase, new()
      where TDAO : DAO<TEntity>
{
    protected TDAO DAO { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogic{TEntity, TDAO}"/> class.
    /// </summary>
    protected BusinessLogic()
    {
        DAO = (TDAO)InstanceFactory.CreateInstance(typeof(TDAO));
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity,bool>> filter = null,
        CancellationToken token = default)
    {
        return await DAO.FindAsync(filter: filter, token: token);
    }

    public async Task<TEntity?> FindByIdAsync(string id,
      CancellationToken token = default)
    {
        return (await FindAsync(filter: f => f.Id == Guid.Parse(id), token: token)).SingleOrDefault();
    }

    public async Task<TEntity> InsertAsync(TEntity entity,
        CancellationToken token = default)
    {
        entity.Id = Guid.NewGuid();
        Expression<Func<TEntity, TEntity>> expr = x => entity;
        return await DAO.InsertAsync(selector: expr, token: token);
    }

    public async Task UpdateAsync(TEntity entity,
       Expression<Func<TEntity, bool>> filter,
       CancellationToken token = default)
    {
        Expression<Func<TEntity, TEntity>> expr = x => entity;
        await DAO.UpdateAsync(selector: expr,
            filter: filter,
            token: token);
    }

    public async Task UpdateByIdAsync(string id,
        TEntity entity,
        CancellationToken token = default)
    {
        await UpdateAsync(entity: entity,
            filter: f => f.Id == Guid.Parse(id),
            token: token);
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> filter,
       CancellationToken token = default)
    {
        await DAO.DeleteAsync(filter: filter,
            token: token);
    }

    public async Task DeleteByIdAsync(string id,
        CancellationToken token = default)
    {
        await DeleteAsync(filter: x => x.Id == Guid.Parse(id),
            token: token);
    }
}

