using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public class IdempotentRemoteFraudCheckService : IFraudCheckService
{
    private readonly IFraudCheckService _nextFraudCheckService;
    private readonly IGetOrderFraudCheckQuery _getOrderFraudCheckQuery;

    public IdempotentRemoteFraudCheckService(IFraudCheckService nextFraudCheckService, IGetOrderFraudCheckQuery getOrderFraudCheckQuery)
    {
        _nextFraudCheckService = nextFraudCheckService;
        _getOrderFraudCheckQuery = getOrderFraudCheckQuery;
    }
    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
       var orderFraudCheckDetails =  _getOrderFraudCheckQuery.Execute(orderId);
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

       return _nextFraudCheckService.Check(orderId, customerOrder);
    }

    private static bool SavedDefaultBypassedFraudCheck(OrderFraudCheckDetails orderFraudCheckDetails)
    {
        return orderFraudCheckDetails.DefaultFraudResult != null;
    }
}