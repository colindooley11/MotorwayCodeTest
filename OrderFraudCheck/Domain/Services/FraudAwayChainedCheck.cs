using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain;

public class FraudAwayChainedCheck : IFraudCheck
{
    private readonly IFraudCheck _fraudCheck;
    private readonly IFraudCheckAway _fraudCheckAway;
    private readonly ISaveOrderFraudCheckDetailsCommand _saveOrderOrderFraudCheckDetailsCommand;
    private readonly decimal _riskScoreThreshold;

    public FraudAwayChainedCheck(IFraudCheck fraudCheck, IFraudCheckAway fraudCheckAway, ISaveOrderFraudCheckDetailsCommand saveOrderOrderFraudCheckDetailsCommand, decimal riskScoreThreshold)
    {
        _fraudCheck = fraudCheck;
        _fraudCheckAway = fraudCheckAway ?? throw new ArgumentNullException(nameof(fraudCheckAway));
        _saveOrderOrderFraudCheckDetailsCommand = saveOrderOrderFraudCheckDetailsCommand ?? throw new ArgumentNullException(nameof(saveOrderOrderFraudCheckDetailsCommand));
        _riskScoreThreshold = riskScoreThreshold;
    }
    public FraudCheckResponseInternal Check(string orderId, CustomerOrder customerOrder)
    {
        var fraudCheckStatus = FraudCheckStatus.DidNotComplete;
        var fraudCheckAwayResponse = _fraudCheckAway.Check(new FraudAwayCheck
        {
            PersonFullName = DeriveFullName(customerOrder),
            PersonAddress = new PersonAddress
            {
                AddressLine1 = customerOrder.CustomerAddress.Line1,
                County = customerOrder.CustomerAddress.Region,
                Town = customerOrder.CustomerAddress.City,
                PostCode = customerOrder.CustomerAddress.PostalCode
            }
        });

        if (fraudCheckAwayResponse.ResponseCode == 200)
        {
            fraudCheckStatus = fraudCheckAwayResponse.FraudRiskScore < _riskScoreThreshold ? FraudCheckStatus.Passed : FraudCheckStatus.Failed;

            _saveOrderOrderFraudCheckDetailsCommand.Execute(fraudCheckAwayResponse, customerOrder);
        }
        else
        {
            fraudCheckStatus = _fraudCheck.Check(orderId, customerOrder).FraudCheckStatus; 
        }
            
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