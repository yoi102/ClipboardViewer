namespace ExceptionHandling;

[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class HandlesExceptionAttribute : Attribute
{
    private readonly Type exceptionType;

    // This is a positional argument
    public HandlesExceptionAttribute(Type exceptionType)
    {
        if (!typeof(Exception).IsAssignableFrom(exceptionType))
        {
            throw new ArgumentException($"Type {exceptionType.FullName} does not inherit from System.Exception.");
        }

        this.exceptionType = exceptionType;
    }

    public Type ExceptionType
    {
        get { return exceptionType; }
    }
}