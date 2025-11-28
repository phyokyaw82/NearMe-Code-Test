using BMS.Core.Data.DbExecutor;
using BMS.Core.Data.ParameterParser;
using BMS.Core.Data.QueryBuilder;
using Microsoft.Extensions.DependencyInjection;
using MSC.Core;
using System.Linq.Expressions;

namespace BMS.Core.Data;

public abstract class DAO<TEntity> where TEntity : 
    EntityBase, new()
{
    private readonly IDbExecutor<TEntity> _executor; 
    private readonly IQueryBuilder<TEntity> _queryBuilder;
    private readonly IParameterParser _parameterParser;
    
    public DAO()
    {
        _executor = DI.Current.BuildServiceProvider().GetService<IDbExecutor<TEntity>>();    
        _queryBuilder = DI.Current.BuildServiceProvider().GetService<IQueryBuilder<TEntity>>();
        _parameterParser = DI.Current.BuildServiceProvider().GetService<IParameterParser>();
    }

    public virtual Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, dynamic>> selector = null,
        Expression<Func<TEntity, bool>> filter = null,
        CancellationToken token = default)
    {
        //var expression = new FilterExpressionVistor<TEntity>(filter);
        //var where = expression.WHERE;
        var sql = _queryBuilder.BuildSelect(selector, filter);
        var parameters = _parameterParser.ExtractAndReplaceParameters(ref sql);
        return _executor.ExecuteReaderAsync(sql, parameters, token);
    }

    public async Task<TEntity> InsertAsync(Expression<Func<TEntity, TEntity>> selector,
        CancellationToken token = default)
    {
        // Build insert SQL with OUTPUT INSERTED.* to return the inserted row
        var sql = _queryBuilder.BuildInsert(selector);
        var parameters = _parameterParser.ExtractAndReplaceParameters(ref sql);

        // Execute and map the result to TEntity
        var insertedEntity = await _executor.QuerySingleAsync(sql, parameters, token);

        return insertedEntity;
    }

    public async Task<int> UpdateAsync(
        Expression<Func<TEntity, TEntity>> selector,
        Expression<Func<TEntity, bool>> filter,
        CancellationToken token = default)
    {
        var sql = _queryBuilder.BuildUpdate(selector, filter);
        var parameters = _parameterParser.ExtractAndReplaceParameters(ref sql);
        return await _executor.ExecuteNonQueryAsync(sql, parameters, token);
    }

    public virtual async Task<int> DeleteAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken token = default)
    {
        var sql = _queryBuilder.BuildDelete(filter);
        var parameters = _parameterParser.ExtractAndReplaceParameters(ref sql);
        return await _executor.ExecuteNonQueryAsync(sql, parameters, token);
    }
}
