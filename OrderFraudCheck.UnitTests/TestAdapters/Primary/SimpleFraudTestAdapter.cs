using MotorwayPaymentsCodeTest;
using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace OrderFraudCheck.UnitTests.TestAdapters;

public class SimpleFraudCheckTestAdapter : ISimpleFraudCheck
{
    private readonly string _result;
    private readonly int _responseCode;

    public SimpleFraudCheckTestAdapter(string result, int responseCode)
    {
        _result = result;
        _responseCode = responseCode;
    }
    
    public SimpleFraudCheckResponse Check(SimpleFraudCheck simpleFraudCheck)
    {
        SimpleFraudCheckDetails = simpleFraudCheck;
        return new SimpleFraudCheckResponse { Result = _result, ResponseCode = _responseCode };
    }

    public SimpleFraudCheck SimpleFraudCheckDetails { get; set; }
}