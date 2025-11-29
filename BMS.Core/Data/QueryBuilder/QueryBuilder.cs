using BMS.Core.Data.QueryVisitors;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace BMS.Core.Data.QueryBuilder;

public class QueryBuilder<TEntity> : IQueryBuilder<TEntity> where TEntity : EntityBase, new()
{
    private readonly string _tableName;

    public QueryBuilder()
    {
        _tableName = typeof(TEntity).GetCustomAttribute<TableAttribute>()?.Name
            ?? typeof(TEntity).Name;
    }

    // ----------------------------------------------------
    // SELECT
    // ----------------------------------------------------
    public string BuildSelect(
        Expression<Func<TEntity, dynamic>> selector = null,
        Expression<Func<TEntity, bool>> filter = null)
    {
        string columns = "*";   // ReaderExpressionVisitor can also be used if you want custom columns

        string where = "";
        if (filter != null)
        {
            var filterVisitor = new FilterExpressionVistor<TEntity>(filter);
            where = " WHERE " + filterVisitor.WHERE.ToString();
        }

        return $"SELECT {columns} FROM {_tableName}{where};";
    }

    // ----------------------------------------------------
    // INSERT
    // ----------------------------------------------------
    public string BuildInsert(Expression<Func<TEntity, TEntity>> selector)
    {
        var writer = new WriterExpressionVisitor<TEntity>(selector);

        var fields = new List<string>();
        var values = new List<string>();

        foreach (var item in writer.FieldCollection)
        {
            fields.Add(item.FieldName);
            values.Add(item.Parameter);  // already SQL literal processed
        }

        // If outputInserted is true, add OUTPUT INSERTED.* to capture inserted row
        var outputClause = "OUTPUT INSERTED.*";

        return $"INSERT INTO {_tableName} ({string.Join(", ", fields)}) {outputClause} VALUES ({string.Join(", ", values)});";
    }

    // ----------------------------------------------------
    // UPDATE
    // ----------------------------------------------------
    public string BuildUpdate(
        Expression<Func<TEntity, TEntity>> selector,
        Expression<Func<TEntity, bool>> filter)
    {
        var writer = new WriterExpressionVisitor<TEntity>(selector);

        var setList = new List<string>();

        foreach (var item in writer.FieldCollection)
        {
            if (item.FieldName == "Id")
            {
                continue;
            } // Skip Id field in update set clause

            setList.Add($"{item.FieldName}={item.Parameter}");
        }

        string where = "";
        if (filter != null)
        {
            var visitor = new FilterExpressionVistor<TEntity>(filter);
            where = " WHERE " + visitor.WHERE;
        }

        return $"UPDATE {_tableName} SET {string.Join(", ", setList)}{where};";
    }

    // ----------------------------------------------------
    // DELETE
    // ----------------------------------------------------
    public string BuildDelete(Expression<Func<TEntity, bool>> filter)
    {
        string where = "";

        if (filter != null)
        {
            var v = new FilterExpressionVistor<TEntity>(filter);
            where = " WHERE " + v.WHERE;
        }

        return $"DELETE FROM {_tableName}{where};";
    }
}

