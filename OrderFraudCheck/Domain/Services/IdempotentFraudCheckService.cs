using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public class IdempotentFraudCheckService : IFraudCheckService
{
    private readonly IFraudCheckService _nextFraudCheckService;
    private readonly IGetOrderFraudCheckQuery _query;

    public IdempotentFraudCheckService(IFraudCheckService nextFraudCheckService, IGetOrderFraudCheckQuery query)
    {
        _nextFraudCheckService = nextFraudCheckService;
        _query = query;
    }
    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
       var orderFraudCheckDetails =  _query.Execute(orderId);
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