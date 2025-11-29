using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace BMS.Core.Extensions;

public static class CommonExtensions
{
    public static string FormatWith(this string instance, params object[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, instance, args);
    }

    public static object CreateDynamic(this Expression expression, Type type)
    {
        return Expression.Lambda<Func<object>>(Expression.Convert(expression, type)).Compile().DynamicInvoke();
    }

    internal static bool IsNullConstant(this Expression expression)
    {
        return expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null;
    }

    public static string RemoveSingelQuote(this string @this)
    {
        string input = @this.ToString().Trim();
        string pattern = "^'(.*)'$";
        string replacement = "$1";
        string result = Regex.Replace(input, pattern, replacement);

        return result;
    }

    public static void Each<T>(this IEnumerable<T> instance, Action<T, int> action)
    {
        int index = 0;
        foreach (T item in instance)
            action(item, index++);
    }

    public static void Each<T>(this IEnumerable<T> instance, Action<T> action)
    {
        if (instance != null)
        {
            foreach (T item in instance)
                action(item);
        }
    }
}
