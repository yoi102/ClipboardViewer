using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Commons.Extensions;

public static class ModuleInitializerExtensions
{
    public static IServiceCollection RunModuleInitializers(this IServiceCollection services,
    IEnumerable<Assembly> assemblies)
    {
        foreach (var asm in assemblies)
        {
            Type[] types = asm.GetTypes();
            var moduleInitializerTypes = types.Where(t => !t.IsAbstract && typeof(IModuleInitializer).IsAssignableFrom(t));
            foreach (var moduleInitializerType in moduleInitializerTypes)
            {
                var initializer = (IModuleInitializer?)Activator.CreateInstance(moduleInitializerType);
                if (initializer == null)
                {
                    throw new ApplicationException($"Cannot create ${moduleInitializerType}");
                }
                initializer.Initialize(services);
            }
        }
        return services;
    }
}