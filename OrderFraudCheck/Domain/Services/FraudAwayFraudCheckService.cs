using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain.Services;

public class FraudAwayFraudCheckService : IFraudCheckService
{
    private readonly IFraudCheckService _nextFraudCheckService;
    private readonly IFraudAwayProvider _fraudAwayProvider;
    private readonly ISaveFraudAwayDetailsCommand _saveFraudAwayDetailsCommand;
    private readonly decimal _riskScoreThreshold;

    public FraudAwayFraudCheckService(IFraudCheckService nextFraudCheckService, IFraudAwayProvider fraudAwayProvider, ISaveFraudAwayDetailsCommand saveFraudAwayDetailsCommand, decimal riskScoreThreshold)
    {
        _nextFraudCheckService = nextFraudCheckService;
        _fraudAwayProvider = fraudAwayProvider ?? throw new ArgumentNullException(nameof(fraudAwayProvider));
        _saveFraudAwayDetailsCommand = saveFraudAwayDetailsCommand ?? throw new ArgumentNullException(nameof(saveFraudAwayDetailsCommand));
        _riskScoreThreshold = riskScoreThreshold;
    }
    public async Task<FraudCheckResponse> Check(string orderId, CustomerOrder customerOrder)
    {
        FraudCheckStatus fraudCheckStatus;
        var fraudAwayResponse = await _fraudAwayProvider.Check(BuildFraudAwayDetails(customerOrder));

        if (ResponseIsSuccessful(fraudAwayResponse))
        {
            fraudCheckStatus = ThenGetTheFraudCheckStatusFromTheRiskScore(fraudAwayResponse);

            await _saveFraudAwayDetailsCommand.Execute(fraudAwayResponse, customerOrder, fraudCheckStatus);

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

    private FraudCheckStatus ThenGetTheFraudCheckStatusFromTheRiskScore(FraudAwayResult fraudCheckAwayResponse)
    {
        return fraudCheckAwayResponse.FraudRiskScore < _riskScoreThreshold ? FraudCheckStatus.Passed : FraudCheckStatus.Failed;
    }

    private static FraudAwayDetails BuildFraudAwayDetails(CustomerOrder customerOrder)
    {
        return new FraudAwayDetails
        {
            PersonFullName = DeriveFullName(customerOrder),
            PersonAddress = new PersonAddress
            {
                AddressLine1 = customerOrder.CustomerAddress.Line1,
                County = customerOrder.CustomerAddress.Region,
                Town = customerOrder.CustomerAddress.City,
                PostCode = customerOrder.CustomerAddress.PostalCode
            }
        };
    }

    private static bool ResponseIsSuccessful(FraudAwayResult fraudCheckAwayResponse)
    {
        return fraudCheckAwayResponse.ResponseCode == 200;
    }

    private static string DeriveFullName(CustomerOrder customerOrder)
    {
        return $"{customerOrder.CustomerAddress.FirstName} {customerOrder.CustomerAddress.LastName}";
    }
}