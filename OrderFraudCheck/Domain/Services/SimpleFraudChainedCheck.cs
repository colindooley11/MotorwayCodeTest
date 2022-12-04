using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain;

public class SimpleFraudChainedCheck : IFraudCheck
{
    private readonly IFraudCheck _fraudCheck;
    private readonly ISimpleFraudCheck _simpleFraudCheck;
    private readonly ISaveOrderFraudCheckSimpleFraudDetailsCommand _saveOrderFraudCheckSimpleFraudDetailsCommand;
    private readonly decimal _riskScoreThreshold;

    public SimpleFraudChainedCheck(IFraudCheck fraudCheck, ISimpleFraudCheck simpleFraudCheck, ISaveOrderFraudCheckSimpleFraudDetailsCommand saveOrderFraudCheckSimpleFraudDetailsCommand)
    {
        _fraudCheck = fraudCheck;
        _simpleFraudCheck = simpleFraudCheck ?? throw new ArgumentNullException(nameof(simpleFraudCheck));
        _saveOrderFraudCheckSimpleFraudDetailsCommand = saveOrderFraudCheckSimpleFraudDetailsCommand ?? throw new ArgumentNullException(nameof(saveOrderFraudCheckSimpleFraudDetailsCommand));
    }
    public FraudCheckResponseInternal Check(string orderId, CustomerOrder customerOrder)
    {
        var fraudCheckStatus = FraudCheckStatus.DidNotComplete;
        var simpleFraudCheckResponse = _simpleFraudCheck.Check(new SimpleFraudCheck
        {
            AddressLine1 = customerOrder.CustomerAddress.Line1,
            Name = DeriveFullName(customerOrder),
            PostCode = customerOrder.CustomerAddress.PostalCode
        });

        if (simpleFraudCheckResponse.ResponseCode == 200)
        {
            fraudCheckStatus = simpleFraudCheckResponse.Result == "Pass"
                ? FraudCheckStatus.Passed
                : FraudCheckStatus.Failed;
        }
            
        _saveOrderFraudCheckSimpleFraudDetailsCommand.Execute(simpleFraudCheckResponse, customerOrder);
            
        return new FraudCheckResponseInternal
        {
            FraudCheckStatus = fraudCheckStatus
        };
    }
        
    private static string DeriveFullName(CustomerOrder customerOrder)
    {
        return $"{customerOrder.CustomerAddress.FirstName} {customerOrder.CustomerAddress.LastName}";
    }
}