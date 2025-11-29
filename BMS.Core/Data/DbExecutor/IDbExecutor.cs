using System.Data.Common;

namespace BMS.Core.Data.DbExecutor;

public interface IDbExecutor<TEntity> where TEntity : EntityBase, new()
{
    Task<IEnumerable<TEntity>> ExecuteReaderAsync(
        string sql,
        IEnumerable<DbParameter> parameters = null,
        CancellationToken token = default);

    Task<int> ExecuteNonQueryAsync(
        string sql,
        IEnumerable<DbParameter> parameters = null,
        CancellationToken token = default);

    Task<object> ExecuteScalarAsync(
      string sql,
      IEnumerable<DbParameter> parameters = null,
      CancellationToken token = default);

    Task<TEntity> QuerySingleAsync(
      string sql,
      IEnumerable<DbParameter> parameters = null,
      CancellationToken token = default);
}
