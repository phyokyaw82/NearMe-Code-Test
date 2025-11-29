using System.Linq.Expressions;

namespace BMS.Core.Data.QueryVisitors;

/// <summary>
/// Expression visitor especially for insert and update statement.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class WriterExpressionVisitor<TEntity> : BaseQueryExpressionVisitor<TEntity> where TEntity : EntityBase
{
    private string _memberName;

    public WriterExpressionVisitor(Expression expression) : base(expression)
    {
        Visit(expression);
    }

    /// <summary>
    /// Runtime field collection.
    /// </summary>
    public ICollection<(string FieldName, string Parameter)> FieldCollection { get; set; } = new List<(string FieldName, string Parameter)>();

    /// <summary>
    /// To transalate Lambda method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        if (node.Body.NodeType == ExpressionType.MemberInit)
            return Visit(node.Body);
        else if (node.Body.NodeType == ExpressionType.MemberAccess)
        {
            var dy = node.Body.CreateDynamic(typeof(object));
            
            dy.GetType().GetProperties().Each(x =>
            {
                FieldCollection.Add((x.Name, $"{DbParameterAnnotation.ConverTo(x.GetValue(dy))}"));
            });
        }
        return node;
    }

    /// <summary>
    /// To read constant value like 1,2,3,name,address ect..
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override Expression VisitConstant(ConstantExpression node)
    {
        FieldCollection.Add((_memberName, $"{DbParameterAnnotation.ConverTo(node.Value)}"));
        return node;
    }

    /// <summary>
    /// To read member exprssion node left side especially eg. MyVariable = 123 for assigning member variable.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
        if (node.BindingType == MemberBindingType.Assignment)
            _memberName = node.Member.Name;

        return base.VisitMemberAssignment(node);
    }

    /// <summary>
    /// To read member expression node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
        {
            _memberName = node.Member.Name;
            return node;
        }

        if (node.Expression != null && node.Expression.NodeType == ExpressionType.MemberAccess
            || node.Expression.NodeType == ExpressionType.Constant)
        {
            var getter = node.CreateDynamic(typeof(object));
            return Visit(Expression.Constant(getter));
        }

        throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));
    }
}