using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters;

public class SaveOrderFraudCheckSimpleFraudDetailsCommandAdapter : ISaveOrderFraudCheckSimpleFraudDetailsCommand
{
    public SimpleFraudCheckResponse Response { get; set; }
    public CustomerOrder Order { get; set; }

    public void Execute(SimpleFraudCheckResponse response, CustomerOrder order)
    {
        Response = response;
        Order = order;
    }
}