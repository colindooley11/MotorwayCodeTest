using OrderFraudCheckCodeTest.UnitTests;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveOrderFraudCheckDetailsCommand
{
    void Execute(FraudCheckAwayResponse response, CustomerOrder order);
}