namespace MotorwayPaymentsCodeTest.Domain.Models;

public class OrderFraudCheckDetails
{
    public CustomerOrder CustomerOrder { get; set; }

    public FraudAwayResult FraudAwayResult {get; set; }
    
    public FraudCheckStatus FraudCheckStatus { get; set; }
}