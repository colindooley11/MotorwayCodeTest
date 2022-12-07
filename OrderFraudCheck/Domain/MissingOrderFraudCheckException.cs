namespace MotorwayPaymentsCodeTest.Domain;

public class MissingOrderFraudCheckException : Exception
{
    public MissingOrderFraudCheckException(string message) :base(message)
    {
    }
}