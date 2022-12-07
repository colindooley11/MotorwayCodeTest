using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface IGetOrderFraudCheckQuery
{
    Task<OrderFraudCheckDetails> Execute(string orderId);
}