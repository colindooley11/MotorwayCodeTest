using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters.Secondary;

public class ByPassThresholdDetailsCommandTestAdapter : ISaveBypassThresholdDetailsCommand
{
    public decimal ByPassAmountThreshold { get; set; }
    public CustomerOrder Order { get; set; }

    public Task Execute(decimal bypassThresholdAmount, CustomerOrder order)
    {
        ByPassAmountThreshold = bypassThresholdAmount;
        Order = order;
        return Task.CompletedTask;
    }
}