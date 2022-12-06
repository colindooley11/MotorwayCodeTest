using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public interface IFraudCheckService
{
    FraudCheckResponse Check(string orderId, CustomerOrder customerOrder);
}