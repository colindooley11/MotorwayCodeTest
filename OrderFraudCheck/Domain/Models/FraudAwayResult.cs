namespace MotorwayPaymentsCodeTest.Domain.Models;

public class FraudAwayResult
{
    public int ResponseCode { get; set; }
    public decimal FraudRiskScore { get; set; }
}