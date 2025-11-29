using BMS.Core.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BMS.Core.Data.QueryVisitors;
/// <summary>
/// Expression where visitor especially to use in where statement.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class FilterExpressionVistor<TEntity> : BaseQueryExpressionVisitor<TEntity> where TEntity : EntityBase
{
    public FilterExpressionVistor(Expression expression)
        : base(expression)
    {
        Visit(expression);
    }

    /// <summary>
    /// To get where statement.
    /// </summary>
    public StringBuilder WHERE { get; set; } = new StringBuilder();

    /// <summary>
    /// Convert binary node like <> != == to well know SQL keywork.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override Expression VisitBinary(BinaryExpression node)
    {
        WHERE.Append("(");

        Visit(node.Left);

        switch (node.NodeType)
        {
            case ExpressionType.And:
                WHERE.Append(" AND ");
                break;

            case ExpressionType.AndAlso:
                WHERE.Append(" AND ");
                break;

            case ExpressionType.Or:
                WHERE.Append(" OR ");
                break;

            case ExpressionType.OrElse:
                WHERE.Append(" OR ");
                break;

            case ExpressionType.Equal:
                if (LIKE)
                    WHERE.Append(" LIKE ");
                else if (node.Right.IsNullConstant())
                    WHERE.Append(" IS ");
                else
                    WHERE.Append(" = ");
                break;

            case ExpressionType.NotEqual:
                if (NOTLIKE)
                    WHERE.Append(" NOT LIKE ");
                else if (node.Right.IsNullConstant())
                    WHERE.Append(" IS NOT ");
                else
                    WHERE.Append(" <> ");
                break;

            case ExpressionType.LessThan:
                WHERE.Append(" < ");
                break;

            case ExpressionType.LessThanOrEqual:
                WHERE.Append(" <= ");
                break;

            case ExpressionType.GreaterThan:
                WHERE.Append(" > ");
                break;

            case ExpressionType.GreaterThanOrEqual:
                WHERE.Append(" >= ");
                break;

            default:
                throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", node.NodeType));
        }

        Visit(node.Right);

        WHERE.Append(")");

        return node;
    }

    /// <summary>
    /// To read constance value especially in RHS eg. MyName="name" 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override Expression VisitConstant(ConstantExpression node)
    {
        var value = node.Value;

        if (LIKE || NOTLIKE)
        {
            var strVal = value?.ToString() ?? "NULL";

            if (STARTWITH)
                WHERE.Append(strVal + "%");
            else if (ENDWITH)
                WHERE.Append("%" + strVal);
            else
                WHERE.Append("%" + strVal + "%");

            LIKE = false;
            NOTLIKE = false;
        }
        else
        {
            if (value == null)
            {
                WHERE.Append("NULL");
            }
            else if (value is string strVal)
            {
                WHERE.Append($"'{strVal.Replace("'", "''")}'"); // escape single quotes
            }
            else if (value is Guid guidVal)
            {
                WHERE.Append($"'{guidVal}'"); // GUID literal in SQL must be quoted
            }
            else if (value.GetType().IsEnum)
            {
                WHERE.Append(Convert.ToInt32(value)); // enum stored as int
            }
            else if (value is bool b)
            {
                WHERE.Append(b ? 1 : 0); // SQL bit
            }
            else
            {
                WHERE.Append(value); // numeric, date, etc.
            }
        }

        return node;
    }

    /// <summary>
    /// To read member variable node especially in LHS eg. MyName="name" .
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override Expression VisitMember(MemberExpression node)
    {
        var nodeType = node.Expression.Type;

        if (!IsRecordType(nodeType))
        {
            var instance = InstanceFactory.CreateInstance(nodeType);
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                WHERE.Append(node.Member.Name);
                return node;
            }
        }

        if (node.Expression != null && node.Expression.NodeType == ExpressionType.MemberAccess
            || node.Expression.NodeType == ExpressionType.Constant)
        {
            var getter = node.CreateDynamic(typeof(object));
            return Visit(Expression.Constant(getter));
        }

        throw new NotSupportedException($"The member '{node.Member.Name}' is not supported.");
    }

    private bool IsRecordType(Type type)
    {
        var isRecord = ((TypeInfo)type).DeclaredProperties.Where(x => x.Name == "EqualityContract")
            .FirstOrDefault()?.GetMethod?.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) is object;
        // isRecord = type.GetMethod("<Clone>$") is object;
        return isRecord;
    }
}

