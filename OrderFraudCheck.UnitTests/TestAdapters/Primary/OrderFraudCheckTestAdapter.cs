using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.PrimaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters.Secondary;

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