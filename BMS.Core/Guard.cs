using BMS.Core.Extensions;

namespace BMS.Core;

public class Guard
{
    /// <summary>
    /// Ensures the specified argument is not null.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    public static void IsNotNull(object parameter, string parameterName)
    {
        if (parameter == null)
            throw new ArgumentNullException(parameterName, Properties.Resources.CannotBeNull.FormatWith(parameterName));
    }

    /// <summary>
    /// Ensures the specified string is not blank.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    public static void IsNotNullOrEmpty(string parameter, string parameterName)
    {
        if (string.IsNullOrEmpty((parameter ?? string.Empty)))
            throw new ArgumentException(Properties.Resources.CannotBeNullOrEmpty.FormatWith(parameterName));
    }

    /// <summary>
    /// Ensures the specified array is not null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    public static void IsNotNullOrEmpty<T>(T[] parameter, string parameterName)
    {
        IsNotNull(parameter, parameterName);

        if (parameter.Length == 0)
            throw new ArgumentException(Properties.Resources.ArrayCannotBeEmpty.FormatWith(parameterName));
    }

    /// <summary>
    /// Ensures the specified collection is not null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    public static void IsNotNullOrEmpty<T>(ICollection<T> parameter, string parameterName)
    {
        IsNotNull(parameter, parameterName);

        if (parameter.Count == 0)
            throw new ArgumentException(Properties.Resources.CollectionCannotBeEmpty.FormatWith(parameterName), parameterName);
    }

    public static void MustBeEndWith(string parameter, string symbol, string parameterName)
    {
        if (!parameter.EndsWith(symbol))
            throw new ArgumentException(Properties.Resources.MustBeEndWith.FormatWith(parameterName, symbol));
    }

    /// <summary>
    /// Ensures the specified value is a positive integer.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    public static void IsNotZeroOrNegative(int parameter, string parameterName)
    {
        if (parameter <= 0)
            throw new ArgumentOutOfRangeException(parameterName, Properties.Resources.CannotBeNegativeOrZero.FormatWith(parameterName));
    }

    /// <summary>
    /// Ensures the specified value is not a negative integer.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    public static void IsNotNegative(int parameter, string parameterName)
    {
        if (parameter < 0)
            throw new ArgumentOutOfRangeException(parameterName, Properties.Resources.CannotBeNegative.FormatWith(parameterName));
    }

    /// <summary>
    /// Ensures the specified value is not a negative integer.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    public static void IsNotNegative(decimal parameter, string parameterName)
    {
        if (parameter < 0)
            throw new ArgumentOutOfRangeException(parameterName, Properties.Resources.CannotBeNegative.FormatWith(parameterName));
    }

    /// <summary>
    /// Ensures the specified value is not a negative float.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    public static void IsNotNegative(float parameter, string parameterName)
    {
        if (parameter < 0)
            throw new ArgumentOutOfRangeException(parameterName, Properties.Resources.CannotBeNegative.FormatWith(parameterName));
    }

    /// <summary>
    /// Ensures the specified collection does not have duplicate key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters">The key collection</param>
    /// <param name="key">match key</param>
    /// <param name="parameterName">Name of the parameter</param>
    public static void IsNotDuplicateKey<T>(ICollection<T> parameters, T key, string parameterName)
    {
        if (parameters.Contains(key))
            throw new DupicateKeyException(parameterName, Properties.Resources.DictionaryKeyShouldNotBeDuplicate.FormatWith(parameterName));
    }

    public static void IsNotEndWith(string key, string source, string parameterName)
    {
        if (!key.EndsWith(source))
            throw new FormatException(Properties.Resources.NotEndWith.FormatWith(parameterName));
    }

    public static void MustKeyContains<Key, Val>(IDictionary<Key, Val> dictionary, Key key)
    {
        if (!dictionary.ContainsKey(key))
        {
            throw new ArgumentException(Properties.Resources.KeyNotContains, key.ToString());
        }
    }
}

