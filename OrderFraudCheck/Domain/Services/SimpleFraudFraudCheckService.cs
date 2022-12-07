using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public class SimpleFraudFraudCheckService : IFraudCheckService
{
    private readonly IFraudCheckService _nextFraudCheckService;
    private readonly ISimpleFraudProvider _simpleFraudProvider;
    private readonly ISaveSimpleFraudDetailsCommand _saveSimpleFraudDetailsCommand;
    private readonly decimal _riskScoreThreshold;

    public SimpleFraudFraudCheckService(IFraudCheckService nextFraudCheckService, ISimpleFraudProvider simpleFraudProvider, ISaveSimpleFraudDetailsCommand saveSimpleFraudDetailsCommand)
    {
        _nextFraudCheckService = nextFraudCheckService;
        _simpleFraudProvider = simpleFraudProvider ?? throw new ArgumentNullException(nameof(simpleFraudProvider));
        _saveSimpleFraudDetailsCommand = saveSimpleFraudDetailsCommand ?? throw new ArgumentNullException(nameof(saveSimpleFraudDetailsCommand));
    }
    public async Task<FraudCheckResponse> Check(string orderId, CustomerOrder customerOrder)
    {
        var simpleFraudCheckResponse = _simpleFraudProvider.Check(new SimpleFraudDetails
        {
            AddressLine1 = customerOrder.CustomerAddress.Line1,
            Name = DeriveFullName(customerOrder),
            PostCode = customerOrder.CustomerAddress.PostalCode
        });

        if (ResponseIsSuccessful(simpleFraudCheckResponse))
        {
            var fraudCheckStatus = ThenGetTheFraudCheckStatusFromTheResponse(simpleFraudCheckResponse);
            
            await _saveSimpleFraudDetailsCommand.Execute(simpleFraudCheckResponse, customerOrder);
            
            return new FraudCheckResponse
            {
                FraudCheckStatus = fraudCheckStatus,
                CustomerGuid = customerOrder.CustomerGuid,
                OrderAmount = customerOrder.OrderAmount,
                OrderId = orderId
            };
        }

        return await _nextFraudCheckService.Check(orderId, customerOrder);
    }

    private static FraudCheckStatus ThenGetTheFraudCheckStatusFromTheResponse(SimpleFraudResult simpleFraudCheckResponse)
    {
        return simpleFraudCheckResponse.Result == "Pass"
            ? FraudCheckStatus.Passed
            : FraudCheckStatus.Failed;
    }

    private static bool ResponseIsSuccessful(SimpleFraudResult simpleFraudCheckResponse)
    {
        return simpleFraudCheckResponse.ResponseCode == 200;
    }

    private static string DeriveFullName(CustomerOrder customerOrder)
    {
        return $"{customerOrder.CustomerAddress.FirstName} {customerOrder.CustomerAddress.LastName}";
    }
}