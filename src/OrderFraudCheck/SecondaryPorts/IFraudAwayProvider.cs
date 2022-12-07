using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface IFraudAwayProvider
{
    public Task<FraudAwayResult?> Check(FraudAwayDetails fraudAwayDetails);
}