namespace WeatherApp.Exceptions;

public class ObjectIsNullException : Exception
{
    public ObjectIsNullException ()
    {}

    public ObjectIsNullException (string message) 
        : base(message)
    {}

    public ObjectIsNullException (string message, Exception innerException)
        : base (message, innerException)
    {}    
}