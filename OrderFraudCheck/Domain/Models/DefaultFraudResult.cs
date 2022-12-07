namespace MotorwayPaymentsCodeTest.Domain.Models;

public class DefaultFraudResult
{
    public decimal BypassThresholdAmount { get; set; }
    public decimal OrderAmount { get; set; }
}