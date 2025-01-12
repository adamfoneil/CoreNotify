namespace CoreNotify.API;

public static class ExceptionExtensions
{
	public static string FullMessage(this Exception exception)
	{
		var result = exception.Message;
		var innerException = exception.InnerException;
		while (innerException != null)
		{
			result += "\r\n -> " + innerException.Message;
			innerException = innerException.InnerException;
		}
		return result;
	}
}
