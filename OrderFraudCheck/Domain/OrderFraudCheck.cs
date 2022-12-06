using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.Domain.Services;
using MotorwayPaymentsCodeTest.PrimaryPorts;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain;

public class OrderFraudCheck : IOrderFraudCheck
{
    private readonly IFraudCheckService _fraudCheckService;
    private readonly IGetOrderFraudCheckQuery _getOrderFraudCheckQuery;

    public OrderFraudCheck(IFraudCheckService fraudCheckService)
    {
        _fraudCheckService = fraudCheckService ?? throw new ArgumentNullException(nameof(fraudCheckService));
    }

    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
        return _fraudCheckService.Check(orderId, customerOrder);
    }
}