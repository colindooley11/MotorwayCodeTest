namespace MotorwayPaymentsCodeTest;

public class FraudCheckResponse
{
    public FraudCheckStatus FraudCheckStatus { get; set; }
    public Guid CustomerGuid { get; set; }
}