using BMS.Core.Data.DbExecutor;
using BMS.Core.Data.ParameterParser;
using BMS.Core.Data.QueryBuilder;
using Microsoft.Extensions.DependencyInjection;
using MSC.Core;

namespace BMS.Core;

public class DependencyContainer : DI
{
    public override void RegisterService(IServiceCollection kernal)
    {
        kernal.AddTransient(typeof(IQueryBuilder<>), typeof(QueryBuilder<>));
        kernal.AddTransient(typeof(IDbExecutor<>), typeof(DbExecutor<>));
        kernal.AddTransient<IParameterParser, ParameterParser>();
    }
}

