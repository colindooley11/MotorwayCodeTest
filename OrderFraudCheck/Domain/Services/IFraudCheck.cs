using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.Domain;

public interface IFraudCheck
{
    FraudCheckResponseInternal Check(string orderId, CustomerOrder customerOrder);
}