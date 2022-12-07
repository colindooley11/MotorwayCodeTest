namespace MotorwayPaymentsCodeTest.Domain.Models;

public class OrderFraudCheckDetails
{
    public CustomerOrder? CustomerOrder { get; set; }
    
    public FraudAwayResult? FraudAwayResult {get; set; }
    
    public SimpleFraudResult? SimpleFraudResult { get; set; }
    
    public DefaultFraudResult? DefaultFraudResult { get; set; }
    
    public FraudCheckStatus FraudCheckStatus { get; set; }
}