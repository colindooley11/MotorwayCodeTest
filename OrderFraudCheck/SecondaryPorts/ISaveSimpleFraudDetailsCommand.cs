using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveSimpleFraudDetailsCommand
{
    Task Execute(SimpleFraudResult response, CustomerOrder order);
}