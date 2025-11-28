using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace BMS.Core.Data.DbExecutor
{
    public class DbExecutor<TEntity> : IDbExecutor<TEntity>
    where TEntity : EntityBase, new()
    {
    #region Public Methods

    // Execute SELECT queries returning multiple rows  
    public async Task<IEnumerable<TEntity>> ExecuteReaderAsync(
        string sql,
        IEnumerable<DbParameter> parameters = null,
        CancellationToken token = default)
        {
            var result = new List<TEntity>();

            using var conn = DaoConfig.ConnectionFactory.CreateConnection();

            if (conn is DbConnection dbConn)
                await dbConn.OpenAsync(token);
            else
                conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            AddParameters(cmd, parameters);

            DbDataReader reader;

            if (cmd is DbCommand dbCmd)
                reader = await dbCmd.ExecuteReaderAsync(token);
            else
                reader = (DbDataReader)cmd.ExecuteReader();

            using (reader)
            {
                while (await reader.ReadAsync(token))
                {
                    result.Add(MapEntity(reader));
                }
            }

            return result;
        }

        // Execute INSERT/UPDATE/DELETE queries  
        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            IEnumerable<DbParameter> parameters = null,
            CancellationToken token = default)
        {
            using var conn = DaoConfig.ConnectionFactory.CreateConnection();

            if (conn is DbConnection dbConn)
                await dbConn.OpenAsync(token);
            else
                conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            AddParameters(cmd, parameters);

            if (cmd is DbCommand dbCmd)
                return await dbCmd.ExecuteNonQueryAsync(token);

            return cmd.ExecuteNonQuery();
        }

        // Execute scalar queries  
        public async Task<object> ExecuteScalarAsync(
            string sql,
            IEnumerable<DbParameter> parameters = null,
            CancellationToken token = default)
        {
            using var conn = DaoConfig.ConnectionFactory.CreateConnection();

            if (conn is DbConnection dbConn)
                await dbConn.OpenAsync(token);
            else
                conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            AddParameters(cmd, parameters);

            if (cmd is DbCommand dbCmd)
                return await dbCmd.ExecuteScalarAsync(token);

            return cmd.ExecuteScalar();
        }

        // NEW: Execute query and return single entity
        public async Task<TEntity> QuerySingleAsync(
            string sql,
            IEnumerable<DbParameter> parameters = null,
            CancellationToken token = default)
        {
            using var conn = DaoConfig.ConnectionFactory.CreateConnection();

            if (conn is DbConnection dbConn)
                await dbConn.OpenAsync(token);
            else
                conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            AddParameters(cmd, parameters);

            DbDataReader reader;

            if (cmd is DbCommand dbCmd)
                reader = await dbCmd.ExecuteReaderAsync(token);
            else
                reader = (DbDataReader)cmd.ExecuteReader();

            using (reader)
            {
                if (await reader.ReadAsync(token))
                {
                    return MapEntity(reader);
                }
            }

            return null; // no record found
        }

        #endregion

        #region Private Helpers  

        private void AddParameters(IDbCommand cmd, IEnumerable<DbParameter> parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                cmd.Parameters.Add(param);
            }
        }

        private TEntity MapEntity(IDataReader reader)
        {
            var entity = new TEntity();

            foreach (var prop in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string columnName = GetColumnName(prop);
                int ordinal;

                try
                {
                    ordinal = reader.GetOrdinal(columnName);
                }
                catch
                {
                    continue;
                }

                if (reader.IsDBNull(ordinal))
                    continue;

                var value = reader.GetValue(ordinal);
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                object convertedValue;

                // ------------------------------
                // ENUM SUPPORT (INT, STRING)
                // ------------------------------
                if (targetType.IsEnum)
                {
                    if (value is string strVal)
                    {
                        convertedValue = Enum.Parse(targetType, strVal, ignoreCase: true);
                    }
                    else
                    {
                        // Always convert numeric to int first → safe for enum backing
                        var intValue = Convert.ToInt32(value);
                        convertedValue = Enum.ToObject(targetType, intValue);
                    }

                    prop.SetValue(entity, convertedValue);
                    continue;
                }

                // ------------------------------
                // GUID SUPPORT
                // ------------------------------
                if (targetType == typeof(Guid))
                {
                    convertedValue = (value is Guid g) ? g : Guid.Parse(value.ToString());
                    prop.SetValue(entity, convertedValue);
                    continue;
                }

                // ------------------------------
                // BOOL SUPPORT (bit / tinyint)
                // ------------------------------
                if (targetType == typeof(bool))
                {
                    if (value is int i) convertedValue = i != 0;
                    else if (value is long l) convertedValue = l != 0;
                    else convertedValue = Convert.ToBoolean(value);

                    prop.SetValue(entity, convertedValue);
                    continue;
                }

                // ------------------------------
                // DATETIME SUPPORT
                // ------------------------------
                if (targetType == typeof(DateTime))
                {
                    convertedValue = Convert.ToDateTime(value);
                    prop.SetValue(entity, convertedValue);
                    continue;
                }

                // ------------------------------
                // FALLBACK: Convert.ChangeType
                // ------------------------------
                convertedValue = Convert.ChangeType(value, targetType);

                prop.SetValue(entity, convertedValue);
            }

            return entity;
        }



        private string GetColumnName(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<ColumnAttribute>();
            return string.IsNullOrWhiteSpace(attr?.Name) ? prop.Name : attr.Name;
        }

        #endregion
    }
}
