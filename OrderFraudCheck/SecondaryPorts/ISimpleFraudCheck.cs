using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISimpleFraudCheck
{
    public SimpleFraudCheckResponse Check(SimpleFraudCheck simpleFraudCheck);
}