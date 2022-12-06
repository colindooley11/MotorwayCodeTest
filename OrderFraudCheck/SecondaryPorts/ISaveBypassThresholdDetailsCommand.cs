using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveBypassThresholdDetailsCommand
{
    void Execute(decimal bypassThresholdAmount, CustomerOrder order);
}