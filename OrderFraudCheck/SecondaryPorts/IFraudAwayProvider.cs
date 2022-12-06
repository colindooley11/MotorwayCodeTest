using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface IFraudAwayProvider
{
    public FraudAwayResult Check(FraudAwayDetails fraudAwayDetails);
}

public interface IFraudProvider<in TDetails,out TVResult>
{
    public TVResult Check(TDetails details);
}