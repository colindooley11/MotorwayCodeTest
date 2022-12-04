using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.PrimaryPorts;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain;

public class OrderFraudCheck : IOrderFraudCheck
{
    private readonly IFraudCheck _fraudCheck;

    public OrderFraudCheck(IFraudCheck fraudCheck)
    {
        _fraudCheck = fraudCheck ?? throw new ArgumentNullException(nameof(fraudCheck));
    }

    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
        var fraudCheckStatus = _fraudCheck.Check(orderId, customerOrder).FraudCheckStatus;

        return new FraudCheckResponse
        {
            FraudCheckStatus = fraudCheckStatus,
            CustomerGuid = customerOrder.CustomerGuid,
            OrderId = orderId,
            OrderAmount = customerOrder.OrderAmount,
        };
    }
}