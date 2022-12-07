using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.PrimaryPorts;

public interface IOrderFraudCheck
{
    Task<FraudCheckResponse?> Check(string orderId, CustomerOrder? customerOrder);
}