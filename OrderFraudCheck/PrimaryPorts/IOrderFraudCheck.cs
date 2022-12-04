using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.PrimaryPorts;

public interface IOrderFraudCheck
{
    FraudCheckResponse Check(string orderId, CustomerOrder customerOrder);
}