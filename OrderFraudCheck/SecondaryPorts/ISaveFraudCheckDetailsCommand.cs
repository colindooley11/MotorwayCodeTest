using OrderFraudCheckCodeTest.UnitTests;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveFraudCheckDetailsCommand
{
    void Execute(FraudProviderResponse response, CustomerOrder order);
}