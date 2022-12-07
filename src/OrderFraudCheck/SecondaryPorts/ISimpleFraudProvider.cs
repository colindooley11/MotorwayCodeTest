using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISimpleFraudProvider
{
    public SimpleFraudResult? Check(SimpleFraudDetails simpleFraudDetails);
}