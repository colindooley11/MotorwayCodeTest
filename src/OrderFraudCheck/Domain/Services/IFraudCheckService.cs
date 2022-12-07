using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public interface IFraudCheckService
{
    Task<FraudCheckResponse?> Check(string orderId, CustomerOrder? customerOrder);
}