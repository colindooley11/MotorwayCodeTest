using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveOrderFraudCheckDetailsCommand
{
    void Execute(FraudCheckAwayResponse response, CustomerOrder order);
}