using Microsoft.Extensions.DependencyInjection;
using MSC.Core;
using System.Data;
using System.Reflection;

namespace BMS.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddBms(this IServiceCollection @this,
            Action<BmsConfigurationOptions> configureOptions)
    {
        Guard.IsNotNull(configureOptions, nameof(configureOptions));

        var options = new BmsConfigurationOptions();
        configureOptions(options);

        Guard.IsNotNull(options.AssemblyNames, nameof(options.AssemblyNames));
        if (options.AssemblyNames.Count == 0)
            throw new ArgumentException("At least one assembly name must be specified.", nameof(options.AssemblyNames));

        DI.Current = @this;

        foreach (var asm in options.AssemblyNames)
        {
            try
            {
                var assembly = Assembly.Load(asm);
                var diTypes = assembly.GetTypes()
                    .Where(x => x.IsClass && x.BaseType == typeof(DI));

                foreach (var type in diTypes)
                {
                    var instance = InstanceFactory.CreateInstance(type.GetTypeInfo());
                    var method = type.GetMethod("RegisterService");
                    method?.Invoke(instance, new object[] { @this });
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Failed to load types from assembly: {asm}");
                foreach (var loaderEx in ex.LoaderExceptions)
                {
                    Console.WriteLine($"LoaderException: {loaderEx.Message}");
                }
                throw; // rethrow or handle as needed
            }
        }
    }
}

