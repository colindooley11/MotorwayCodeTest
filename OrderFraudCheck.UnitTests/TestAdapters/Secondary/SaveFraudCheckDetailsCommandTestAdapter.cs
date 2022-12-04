using MotorwayPaymentsCodeTest;
using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.PrimaryPorts;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters;

public class SaveOrderFraudCheckAwayDetailsCommandAdapter : ISaveOrderFraudCheckDetailsCommand
{
    public FraudCheckAwayResponse Response { get; set; }
    public CustomerOrder Order { get; set; }

    public void Execute(FraudCheckAwayResponse response, CustomerOrder order)
    {
        Response = response;
        Order = order;
    }
}

public class OrderFraudCheckTestAdapter : IOrderFraudCheck
{
    private readonly MotorwayPaymentsCodeTest.Domain.OrderFraudCheck _fraudCheck;

    public OrderFraudCheckTestAdapter(MotorwayPaymentsCodeTest.Domain.OrderFraudCheck fraudCheck)
    {
        _fraudCheck = fraudCheck;
    }
    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
       return _fraudCheck.Check(orderId, customerOrder);
    }
}