using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters.Secondary;

public class ByPassThersholdDetailsCommandTestAdapter : ISaveBypassThresholdDetailsCommand
{
    public decimal ByPassAmountThreshold { get; set; }
    public CustomerOrder Order { get; set; }

    public void Execute(decimal bypassThresholdAmount, CustomerOrder order)
    {
        ByPassAmountThreshold = bypassThresholdAmount;
        Order = order;
    }
}