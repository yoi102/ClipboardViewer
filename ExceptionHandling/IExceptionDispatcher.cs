namespace ExceptionHandling;

public interface IExceptionDispatcher
{
    Task<bool> DispatchAsync(Exception exception);
}