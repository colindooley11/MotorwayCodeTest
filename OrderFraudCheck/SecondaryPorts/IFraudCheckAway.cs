using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface IFraudCheckAway
{
    public FraudCheckAwayResponse Check(FraudAwayCheck fraudAwayCheck);
}