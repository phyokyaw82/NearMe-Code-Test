using BMS.Api.Validators;
using MSC.Core;
namespace BMS.Api;

public class DependencyContainer : DI
{
    public override void RegisterService(IServiceCollection kernal)
    {
        kernal.AddTransient<BookValidator>();
    }
}

