using BMS.Core.Extensions;

namespace BMS.Core.Data.QueryVisitors;

public class DbParameterAnnotation
{
    private const char symbol = '^';
    private const char seperator = '.';

    /// <summary>
    /// Convert to <##> symbol as parameter and ^ symbol as non parameter annotation. 
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static string ConverTo(object parameter)
    {
        if (parameter == null)
        {
            return "NULL";
        }

        Type paramType = parameter.GetType();

        // Check for enum or nullable enum
        if (paramType.IsEnum || Nullable.GetUnderlyingType(paramType)?.IsEnum == true)
        {
            return ((int)Convert.ChangeType(parameter, Enum.GetUnderlyingType(Nullable.GetUnderlyingType(paramType) ?? paramType))).ToString();
        }

        string paramStr = parameter.ToString();

        if (string.Equals(paramStr, "NULL", StringComparison.OrdinalIgnoreCase))
        {
            return "NULL";
        }

        if (paramStr.StartsWith(symbol.ToString()))
        {
            return paramStr.Trim().TrimStart(symbol);
        }

        return $"'{paramStr.RemoveSingelQuote()}'";
    }
}

