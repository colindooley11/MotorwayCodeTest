namespace OrderFraudCheck.UnitTests.TestSetup;

public static class BddfyExceptionExtensions
{
    public static async Task<Exception?> ExecuteActionThatThrows(Func<Task> action)
    {
        Exception? exception = null;
        try
        {
            await action();
        }
        catch (Exception? ex)
        {
            exception = ex;
        }

        return exception;
    }
}