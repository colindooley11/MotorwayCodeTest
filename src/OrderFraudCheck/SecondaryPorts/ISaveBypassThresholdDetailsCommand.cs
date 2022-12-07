using MotorwayPaymentsCodeTest.Domain.Models;

namespace MotorwayPaymentsCodeTest.SecondaryPorts;

public interface ISaveBypassThresholdDetailsCommand
{
    Task Execute(decimal bypassThresholdAmount, CustomerOrder? order);
}