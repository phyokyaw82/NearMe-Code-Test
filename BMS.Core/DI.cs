namespace MSC.Core
{
    using Microsoft.Extensions.DependencyInjection;

    public abstract class DI
    {
        public DI() { }

        public abstract void RegisterService(IServiceCollection kernal);

        public static IServiceCollection Current
        {
            get;
            internal set;
        }
    }
}
