using MotorwayPaymentsCodeTest;

namespace OrderFraudCheck.UnitTests.TestAdapters;

public class SimpleFraudCheckTestAdapter : ISimpleFraudCheck
{
    private readonly int _responseCode;

    public SimpleFraudCheckTestAdapter(int responseCode)
    {
        _responseCode = responseCode;
    }
    
    public SimpleFraudCheckResponse Check(SimpleFraudCheck simpleFraudCheck)
    {
        SimpleFraudCheckDetails = simpleFraudCheck;
        return new SimpleFraudCheckResponse() { ResponseCode = _responseCode };
    }

    public SimpleFraudCheck SimpleFraudCheckDetails { get; set; }
}