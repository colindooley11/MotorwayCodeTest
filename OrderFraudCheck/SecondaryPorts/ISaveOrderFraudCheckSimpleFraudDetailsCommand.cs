using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveOrderFraudCheckSimpleFraudDetailsCommand
{
    void Execute(SimpleFraudCheckResponse response, CustomerOrder order);
}