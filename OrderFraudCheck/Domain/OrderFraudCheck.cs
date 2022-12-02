using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheckCodeTest.UnitTests;

namespace MotorwayPaymentsCodeTest;

public class OrderFraudCheck
{
    private readonly IFraudCheckAway _fraudCheckAway;
    private readonly ISimpleFraudCheck _simpleFraudCheck;
    private readonly ISaveOrderFraudCheckDetailsCommand _saveOrderOrderFraudCheckDetailsCommand;
    private readonly decimal _riskScoreThreshold;

    public OrderFraudCheck(IFraudCheckAway fraudCheckAway, ISimpleFraudCheck simpleFraudCheck,
        ISaveOrderFraudCheckDetailsCommand command, decimal riskScoreThreshold)
    {
        _fraudCheckAway = fraudCheckAway ?? throw new ArgumentNullException(nameof(fraudCheckAway));
        _simpleFraudCheck = simpleFraudCheck ?? throw new ArgumentNullException(nameof(simpleFraudCheck));
        _saveOrderOrderFraudCheckDetailsCommand = command ?? throw new ArgumentNullException(nameof(command));
        _riskScoreThreshold = riskScoreThreshold;
    }

    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
        var fraudCheckAwayResponse = _fraudCheckAway.Check(new FraudAwayCheck
        {
            PersonFullName = FullName(customerOrder),
            PersonAddress = new PersonAddress
            {
                AddressLine1 = customerOrder.CustomerAddress.Line1,
                County = customerOrder.CustomerAddress.Region,
                Town = customerOrder.CustomerAddress.City,
                PostCode = customerOrder.CustomerAddress.PostalCode
            }
        });

        if (fraudCheckAwayResponse.ResponseCode != 200)
        {
            var simpleFraudCheckResponse = _simpleFraudCheck.Check(new SimpleFraudCheck
            {
                AddressLine1 = customerOrder.CustomerAddress.Line1,
                Name = FullName(customerOrder),
                PostCode = customerOrder.CustomerAddress.PostalCode
            });
        }

        
        var fraudCheckStatus = FraudCheckStatus.Passed;
        if (fraudCheckAwayResponse.FraudRiskScore > _riskScoreThreshold)
        {
            fraudCheckStatus = FraudCheckStatus.Failed;
        }
        
        _saveOrderOrderFraudCheckDetailsCommand.Execute(fraudCheckAwayResponse, customerOrder);
            
        return new FraudCheckResponse
        {
            FraudCheckStatus = fraudCheckStatus, 
            CustomerGuid  = customerOrder.CustomerGuid,
            OrderId = orderId,
            OrderAmount = customerOrder.OrderAmount,
        };
    }

    private static string FullName(CustomerOrder customerOrder)
    {
        return $"{customerOrder.CustomerAddress.FirstName} {customerOrder.CustomerAddress.LastName}";
    }
}