using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters.Secondary;

public class SimpleFraudProviderTestAdapter : ISimpleFraudProvider
{
    private readonly string? _result;
    private readonly int _responseCode;

    public SimpleFraudProviderTestAdapter(string? result, int responseCode)
    {
        _result = result;
        _responseCode = responseCode;
    }
    
    public SimpleFraudResult? Check(SimpleFraudDetails simpleFraudDetails)
    {
        SimpleFraudDetails = simpleFraudDetails;
        return new SimpleFraudResult { Result = _result, ResponseCode = _responseCode };
      
    }

    public SimpleFraudDetails SimpleFraudDetails { get; set; } = null!;
}
