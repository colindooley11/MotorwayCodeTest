using MotorwayPaymentsCodeTest;
using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheckCodeTest.UnitTests;

namespace OrderFraudCheck.UnitTests.TestAdapters;

public class SaveOrderFraudCheckDetailsCommandAdapter : ISaveOrderFraudCheckDetailsCommand
{
    public FraudCheckAwayResponse Response { get; set; }
    public CustomerOrder Order { get; set; }

    public void Execute(FraudCheckAwayResponse response, CustomerOrder order)
    {
        Response = response;
        Order = order;
    }
}