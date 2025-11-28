using System.Linq.Expressions;

namespace BMS.Core.Data.QueryVisitors;

/// <summary>
/// Base of query Expression visitor that emit sql statement. 
///</summary>
    public abstract class BaseQueryExpressionVisitor<TEntity> : ExpressionVisitor where TEntity : EntityBase
{
    public bool LIKE { get; set; }
    public bool NOTLIKE { get; set; }
    public bool STARTWITH { get; private set; }
    public bool ENDWITH { get; private set; }

    public BaseQueryExpressionVisitor(Expression expression)
    {
        Entity = (TEntity)InstanceFactory.CreateInstance(typeof(TEntity)); // Activator.CreateInstance<TEntity>();
    }

    /// <summary>
    /// Composing entity.
    /// </summary>
    protected TEntity Entity { get; private set; }

    /// <summary>
    /// To read custom method like .ToString(), .Equals(), MyAnyMethod() ect.. and complie at run time to collect return value.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        LIKE = false;
        STARTWITH = false;
        ENDWITH = false;

        if (node.Method.DeclaringType == typeof(string) && node.Method.Name == "Equals")
        {
            if (NOTLIKE)
            {
                Visit(Expression.NotEqual(node.Object, node.Arguments[0]));
                NOTLIKE = false;
            }
            else
            {
                Visit(Expression.Equal(node.Object, node.Arguments[0]));
            }
            return node;
        }
        else if (node.Method.Name.Equals("Contains", StringComparison.OrdinalIgnoreCase))
        {
            if (NOTLIKE)
            {
                Visit(Expression.NotEqual(node.Object, node.Arguments[0]));
                NOTLIKE = false;
            }
            else
            {
                LIKE = true;
                Visit(Expression.Equal(node.Object, node.Arguments[0]));
            }
            return node;
        }
        else if (node.Method.Name.Equals("StartsWith", StringComparison.OrdinalIgnoreCase))
        {
            STARTWITH = true;

            if (NOTLIKE)
            {
                Visit(Expression.NotEqual(node.Object, node.Arguments[0]));
                NOTLIKE = false;
            }
            else
            {
                LIKE = true;
                Visit(Expression.Equal(node.Object, node.Arguments[0]));
            }
            return node;
        }
        else if (node.Method.Name.Equals("EndsWith", StringComparison.OrdinalIgnoreCase))
        {
            ENDWITH = true;

            if (NOTLIKE)
            {
                Visit(Expression.NotEqual(node.Object, node.Arguments[0]));
                NOTLIKE = false;
            }
            else
            {
                LIKE = true;
                Visit(Expression.Equal(node.Object, node.Arguments[0]));
            }
            return node;
        }
        else if (!string.IsNullOrEmpty(node.Method.Name))
        {
            var getter = node.CreateDynamic(typeof(object));
            Visit(Expression.Constant(getter));
            return node;
        }

        throw new NotSupportedException($"The method '{node.Method.Name}' is not supported.");
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Not)
            NOTLIKE = true;

        return base.VisitUnary(node);
    }
}
