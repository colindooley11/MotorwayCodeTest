using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.PrimaryPorts;

public interface IOrderFraudCheckQuery
{
    Task<FraudCheckResponse?> Get(string orderId);
}