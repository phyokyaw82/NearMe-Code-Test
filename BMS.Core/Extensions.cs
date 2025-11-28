using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace BMS.Core;

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
        return (expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null);
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


//    public static void AddBms(this IServiceCollection @this,
//          Action<MscConfigOptions> configureOptions)
//    {
//        Guard.IsNotNull(configureOptions, nameof(configureOptions));

//        var options = new MscConfigOptions();
//        configureOptions(options);

//        Guard.IsNotNull(options.AssemblyNames, nameof(options.AssemblyNames));
//        if (options.AssemblyNames.Count == 0)
//            throw new ArgumentException("At least one assembly name must be specified.", nameof(options.AssemblyNames));

//        DI.Current = @this;

//        foreach (var asm in options.AssemblyNames)
//        {
//            try
//            {
//                var assembly = Assembly.Load(asm);
//                var diTypes = assembly.GetTypes()
//                    .Where(x => x.IsClass && x.BaseType == typeof(DI));

//                foreach (var type in diTypes)
//                {
//                    var instance = InstanceFactory.CreateInstance(type.GetTypeInfo());
//                    var method = type.GetMethod("RegisterService");
//                    method?.Invoke(instance, new object[] { @this });
//                }
//            }
//            catch (ReflectionTypeLoadException ex)
//            {
//                Console.WriteLine($"Failed to load types from assembly: {asm}");
//                foreach (var loaderEx in ex.LoaderExceptions)
//                {
//                    Console.WriteLine($"LoaderException: {loaderEx.Message}");
//                }
//                throw; // rethrow or handle as needed
//            }
//        }

//        var appSettingInstance = new MscAppServices();
//        var dataServicesInstance = new MscDataServices();

//        options.ConfigureAppServices?.Invoke(appSettingInstance);
//        options.ConfigureDataServices?.Invoke(dataServicesInstance);


//        // Register singletons for your service containers
//        @this.AddSingleton(appSettingInstance);
//        @this.AddSingleton(dataServicesInstance);

//        // Register background queue & hosted service (singleton + hosted service)
//        @this.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
//        @this.AddHostedService<QueuedHostedService>();

//        @this.AddScoped<IMscAppRepository>(provider => appSettingInstance);
//        @this.AddScoped<IMscDataRepository>(provider => dataServicesInstance);
//    }

