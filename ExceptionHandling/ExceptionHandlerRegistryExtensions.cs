using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ExceptionHandling;

public static class ExceptionHandlerRegistryExtensions
{
    public static void RegisterHandlers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var handlerTypes = assemblies.SelectMany(a =>
        {
            try
            {
                return a.GetTypes();
            }
            catch
            {
                return Array.Empty<Type>(); // 某些程序集反射失败要跳过
            }
        }).Where(t =>
                typeof(IExceptionHandler).IsAssignableFrom(t) &&
                !t.IsInterface &&
                !t.IsAbstract);
        var map = new Dictionary<Type, Type>();

        foreach (var type in handlerTypes)
        {
            var attr = type.GetCustomAttribute<HandlesExceptionAttribute>();
            if (attr == null) continue;

            var exceptionType = attr.ExceptionType;

            if (map.ContainsKey(exceptionType))
            {
                // 如果已经存在相同的异常类型处理器，则抛出异常
                // 一个异常类型只能有一个处理器！！！！
                throw new InvalidOperationException(
                    $"Duplicate exception handler found for exception type {exceptionType.FullName}.");
            }

            map[exceptionType] = type;
            services.AddTransient(type); // 注册处理器到 DI 容器
        }
        var exceptionHandlerMap = new ReadOnlyDictionary<Type, Type>(map);
        ExceptionDispatcher.SetExceptionHandlerMap(exceptionHandlerMap);
        services.AddSingleton<IExceptionDispatcher, ExceptionDispatcher>();
    }
}