using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.PrimaryPorts;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain;

public class OrderFraudCheckQuery : IOrderFraudCheckQuery
{
    private readonly IGetOrderFraudCheckQuery _getOrderFraudCheckQuery;

    public OrderFraudCheckQuery(IGetOrderFraudCheckQuery getOrderFraudCheckQuery)
    {
        _getOrderFraudCheckQuery = getOrderFraudCheckQuery;
    }
   
    public async Task<FraudCheckResponse> Get(string orderId)
    {
        var orderFraudCheckDetails =  await _getOrderFraudCheckQuery.Execute(orderId);
        if (orderFraudCheckDetails != null)
        {
            if (SavedDefaultBypassedFraudCheck(orderFraudCheckDetails))
            {
                var fraudCheckStatus =
                    orderFraudCheckDetails.DefaultFraudResult.OrderAmount <= orderFraudCheckDetails.DefaultFraudResult.BypassThresholdAmount
                        ? FraudCheckStatus.Passed
                        : FraudCheckStatus.Failed;
               
                return new FraudCheckResponse
                {
                    FraudCheckStatus = fraudCheckStatus,
                    CustomerGuid = orderFraudCheckDetails.CustomerOrder.CustomerGuid,
                    OrderAmount = orderFraudCheckDetails.DefaultFraudResult.OrderAmount,
                    OrderId = orderId
                };
            }
           
            return new FraudCheckResponse
            {
                FraudCheckStatus = orderFraudCheckDetails.FraudCheckStatus,
                CustomerGuid = orderFraudCheckDetails.CustomerOrder.CustomerGuid,
                OrderAmount = orderFraudCheckDetails.CustomerOrder.OrderAmount,
                OrderId = orderId
            };
        }
        
        return null;

    }
    
    private static bool SavedDefaultBypassedFraudCheck(OrderFraudCheckDetails orderFraudCheckDetails)
    {
        return orderFraudCheckDetails.DefaultFraudResult != null;
    }
}