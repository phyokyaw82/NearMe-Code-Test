using BMS.Domain.BusinessLogic;
using BMS.Domain.Repository;
using Microsoft.Extensions.DependencyInjection;
using MSC.Core;

namespace BMS.Domain;

public class DependencyContainer : DI
{
    public override void RegisterService(IServiceCollection kernal)
    {
        kernal.AddTransient<IBookLogic, Book>();
        kernal.AddTransient<IDbContext, DataContext>();
    }
}

