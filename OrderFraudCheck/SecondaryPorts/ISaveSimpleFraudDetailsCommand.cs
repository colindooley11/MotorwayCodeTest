using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveSimpleFraudDetailsCommand
{
    void Execute(SimpleFraudResult response, CustomerOrder order);
}