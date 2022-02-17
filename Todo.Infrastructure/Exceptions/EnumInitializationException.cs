namespace Todo.Infrastructure.Exceptions;

public class EnumInitializationException : InvalidOperationException
{
    public EnumInitializationException(string message) : base(message)
    {

    }
}
