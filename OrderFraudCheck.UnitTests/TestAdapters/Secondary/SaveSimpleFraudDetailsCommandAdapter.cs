using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters.Secondary;

public class SaveSimpleFraudDetailsCommandAdapter : ISaveSimpleFraudDetailsCommand
{
    public SimpleFraudResult Response { get; set; }
    public CustomerOrder Order { get; set; }

    public void Execute(SimpleFraudResult response, CustomerOrder order)
    {
        Response = response;
        Order = order;
    }
}