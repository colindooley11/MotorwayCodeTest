using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveFraudAwayDetailsCommand
{
    void Execute(FraudAwayResult result, CustomerOrder order, FraudCheckStatus status);
}
