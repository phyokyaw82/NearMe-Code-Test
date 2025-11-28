using System.Data.Common;

namespace BMS.Core.Data.ParameterParser;

public interface IParameterParser
{
    IList<DbParameter> ExtractAndReplaceParameters(ref string sqlQuery);
}

