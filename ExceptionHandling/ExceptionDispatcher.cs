using System.Collections.ObjectModel;

namespace ExceptionHandling;

internal class ExceptionDispatcher : IExceptionDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private static ReadOnlyDictionary<Type, Type> _exceptionHandlerMap = null!;

    public ExceptionDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal static void SetExceptionHandlerMap(ReadOnlyDictionary<Type, Type> exceptionHandlerMap)
    {
        _exceptionHandlerMap = exceptionHandlerMap ??
            throw new ArgumentNullException(nameof(exceptionHandlerMap), "Exception handler map cannot be null.");
    }

    public async Task<bool> DispatchAsync(Exception exception)
    {
        var exType = exception.GetType();

        // 支持多态匹配（Optional）
        var handlerType = _exceptionHandlerMap.FirstOrDefault(kv => kv.Key.IsAssignableFrom(exType)).Value;

        //则尝试匹配对应的处理器类型
        if (handlerType is null)
            return false;

        var handler = _serviceProvider.GetService(handlerType)!;
        if (handler is not IExceptionHandler exceptionHandler)
            return false;

        return await exceptionHandler.HandleExceptionAsync(exception);
    }
}