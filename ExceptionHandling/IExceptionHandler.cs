namespace ExceptionHandling;

public interface IExceptionHandler
{
    Task<bool> HandleExceptionAsync(Exception exception);
}