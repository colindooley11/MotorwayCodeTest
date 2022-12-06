using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public class DefaultFraudBypassFraudCheckService : IFraudCheckService
{
    private readonly ISaveBypassThresholdDetailsCommand _saveBypassThresholdDetailsCommand;
    private readonly decimal _bypassThresholdAmount;

    public DefaultFraudBypassFraudCheckService(ISaveBypassThresholdDetailsCommand command,
        decimal bypassThresholdAmount)
    {
        _saveBypassThresholdDetailsCommand = command ?? throw new ArgumentNullException(nameof(command));
        _bypassThresholdAmount = bypassThresholdAmount;
    }

    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
        _saveBypassThresholdDetailsCommand.Execute(_bypassThresholdAmount, customerOrder);
        return new FraudCheckResponse
        {
            FraudCheckStatus = customerOrder.OrderAmount <= _bypassThresholdAmount ? FraudCheckStatus.Passed : FraudCheckStatus.Failed,
            CustomerGuid = customerOrder.CustomerGuid,
            OrderAmount = customerOrder.OrderAmount,
            OrderId = orderId
        };
    }
}
