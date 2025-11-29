using System.Linq.Expressions;

namespace BMS.Core.Extensions;

public static class ExpressionExtensions
{
    public static object CreateDynamic(this Expression expression, Type type)
    {
        return Expression.Lambda<Func<object>>(Expression.Convert(expression, type)).Compile().DynamicInvoke();
    }

    internal static bool IsNullConstant(this Expression expression)
    {
        return expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null;
    }
}

