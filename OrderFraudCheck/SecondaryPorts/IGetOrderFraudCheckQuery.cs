using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface IGetOrderFraudCheckQuery
{
    OrderFraudCheckDetails Execute(string orderId);
}