using OrderFraudCheck.UnitTests.TestAdapters;

namespace MotorwayPaymentsCodeTest;

public interface ISimpleFraudCheck
{
    public SimpleFraudCheckResponse Check(SimpleFraudCheck simpleFraudCheck);
}