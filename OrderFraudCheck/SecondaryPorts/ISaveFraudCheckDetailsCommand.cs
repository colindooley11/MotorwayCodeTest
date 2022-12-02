using OrderFraudCheckCodeTest.UnitTests;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveFraudCheckDetailsCommand
{
    void Execute(FraudCheckResponse response, CustomerOrder order);
}