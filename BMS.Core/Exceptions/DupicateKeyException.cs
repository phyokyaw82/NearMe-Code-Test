using BMS.Core.Extensions;

namespace BMS.Core;

public class DupicateKeyException : Exception
{
    private string _errorMessage;
    private string _parameterName;

    public DupicateKeyException(string parameterName, string errorMessage)
    {
        _errorMessage = errorMessage ?? Properties.Resources.DictionaryKeyShouldNotBeDuplicate.FormatWith(parameterName);
        _parameterName = parameterName;
    }

    public override string StackTrace { get { return _parameterName; } }

    public override string Message { get { return _errorMessage; } }
}