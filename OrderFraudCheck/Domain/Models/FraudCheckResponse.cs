namespace MotorwayPaymentsCodeTest.Domain.Models;

public class FraudCheckResponse
{
    public FraudCheckStatus FraudCheckStatus { get; set; }
    public Guid CustomerGuid { get; set; }
    public string OrderId { get; set; }
    public decimal OrderAmount { get; set; }
}

public class FraudCheckResponseInternal
{
    public bool DidItsThing { get; set; }

    public FraudCheckStatus  FraudCheckStatus { get; set; }
    public FraudCheckResponse response { get; set; }
}