using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveFraudAwayDetailsCommand
{
    Task Execute(FraudAwayResult result, CustomerOrder order, FraudCheckStatus status);
}
