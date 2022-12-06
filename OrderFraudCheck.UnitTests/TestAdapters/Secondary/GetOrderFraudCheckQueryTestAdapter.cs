using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters.Secondary;

public class GetOrderFraudCheckQueryTestAdapter : IGetOrderFraudCheckQuery
{
    private readonly OrderFraudCheckDetails _orderFraudCheckDetails;

    public GetOrderFraudCheckQueryTestAdapter(OrderFraudCheckDetails orderOrderFraudCheckDetails)
    {
        _orderFraudCheckDetails = orderOrderFraudCheckDetails;
    }
    public OrderFraudCheckDetails Execute(string orderId)
    {
        return _orderFraudCheckDetails; 
    }
}