using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters.Secondary;

public class SaveFraudAwayDetailsCommandTestAdapter : ISaveFraudAwayDetailsCommand
{
    public SaveFraudAwayDetailsCommandTestAdapter()
    {
    }
    
    public FraudAwayResult Response { get; set; }
    public CustomerOrder Order { get; set; }
    public FraudCheckStatus Status { get; set; }

    public void Execute(FraudAwayResult result, CustomerOrder order, FraudCheckStatus status)
    {
        Response = result;
        Order = order;
        Status = status;
    }
}