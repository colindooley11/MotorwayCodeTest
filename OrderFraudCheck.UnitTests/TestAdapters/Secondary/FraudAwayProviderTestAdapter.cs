using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters.Primary;

public class FraudAwayProviderTestAdapter : IFraudAwayProvider
{
    private readonly decimal _fraudRiskScore;
    private readonly int _responseCode;

    public FraudAwayProviderTestAdapter(decimal fraudRiskScore, int responseCode)
    {
        _fraudRiskScore = fraudRiskScore;
        _responseCode = responseCode;
    }

    public FraudAwayResult Check(FraudAwayDetails fraudAwayDetails)
    {
        FraudAwayDetails = fraudAwayDetails;
        return new FraudAwayResult { FraudRiskScore = _fraudRiskScore, ResponseCode = _responseCode };
    }

    public FraudAwayDetails FraudAwayDetails { get; set; }
}