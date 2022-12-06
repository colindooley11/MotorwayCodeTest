using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public class IdempotentFraudCheckService : IFraudCheckService
{
    private readonly IFraudCheckService _nextFraudCheckService;
    private readonly IGetOrderFraudCheckQuery _getOrderFraudCheckQuery;

    public IdempotentFraudCheckService(IFraudCheckService nextFraudCheckService, IGetOrderFraudCheckQuery getOrderFraudCheckQuery)
    {
        _nextFraudCheckService = nextFraudCheckService;
        _getOrderFraudCheckQuery = getOrderFraudCheckQuery;
    }
    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
       var orderFraudCheckDetails =  _getOrderFraudCheckQuery.Execute(orderId);
       if (orderFraudCheckDetails != null)
       {
           return new FraudCheckResponse
           {
               FraudCheckStatus = orderFraudCheckDetails.FraudCheckStatus,
               CustomerGuid = orderFraudCheckDetails.CustomerOrder.CustomerGuid,
               OrderAmount = orderFraudCheckDetails.CustomerOrder.OrderAmount,
               OrderId = orderId
           };
       }

       return _nextFraudCheckService.Check(orderId, customerOrder);
    }
}