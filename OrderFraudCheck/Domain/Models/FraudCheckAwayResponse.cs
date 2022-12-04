namespace MotorwayPaymentsCodeTest.Domain.Models;

public class FraudCheckAwayResponse
{
    public int ResponseCode { get; set; }
    public decimal FraudRiskScore { get; set; }
}