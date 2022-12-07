using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public class DefaultFraudBypassFraudCheckService : IFraudCheckService
{
    private readonly ISaveBypassThresholdDetailsCommand _saveBypassThresholdDetailsCommand;
    private readonly ByPassOptions _bypassOptions;

    public DefaultFraudBypassFraudCheckService(ISaveBypassThresholdDetailsCommand command, ByPassOptions bypassOptions)
    {
        _saveBypassThresholdDetailsCommand = command ?? throw new ArgumentNullException(nameof(command));
        _bypassOptions = bypassOptions ?? throw new ArgumentNullException(nameof(bypassOptions));
    }

    public  async Task<FraudCheckResponse?> Check(string orderId, CustomerOrder? customerOrder)
    {
        await _saveBypassThresholdDetailsCommand.Execute(_bypassOptions.BypassThresholdAmount, customerOrder);
        return new FraudCheckResponse
        {
            FraudCheckStatus = customerOrder!.OrderAmount <= _bypassOptions.BypassThresholdAmount ? FraudCheckStatus.Passed : FraudCheckStatus.Failed,
            CustomerGuid = customerOrder.CustomerGuid,
            OrderAmount = customerOrder.OrderAmount,
            OrderId = orderId
        };
    }
}