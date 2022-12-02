using MotorwayPaymentsCodeTest;
using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheckCodeTest.UnitTests;

namespace OrderFraudCheck.UnitTests.TestAdapters;

public class SaveFraudCheckDetailsCommandAdapter : ISaveFraudCheckDetailsCommand
{
    public FraudCheckResponse Response { get; set; }
    public CustomerOrder Order { get; set; }

    public void Execute(FraudCheckResponse response, CustomerOrder order)
    {
        Response = response;
        Order = order;
    }
}