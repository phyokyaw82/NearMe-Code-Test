using System.Globalization;
using System.Text.RegularExpressions;

namespace BMS.Core.Extensions;

public static class StringExtensions
{
    public static string FormatWith(this string instance, params object[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, instance, args);
    }

    public static string RemoveSingelQuote(this string @this)
    {
        string input = @this.ToString().Trim();
        string pattern = "^'(.*)'$";
        string replacement = "$1";
        string result = Regex.Replace(input, pattern, replacement);

        return result;
    }
}
