using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheckCodeTest.UnitTests;

namespace MotorwayPaymentsCodeTest;

public class OrderFraudCheck
{
    private readonly IFraudCheckAway _fraudCheckAway;
    private readonly ISaveFraudCheckDetailsCommand _command;
    private readonly decimal _riskScoreThreshold;

    public OrderFraudCheck(IFraudCheckAway fraudCheckAway, ISaveFraudCheckDetailsCommand command, decimal riskScoreThreshold)
    {
        _fraudCheckAway = fraudCheckAway ?? throw new ArgumentNullException(nameof(fraudCheckAway));
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _riskScoreThreshold = riskScoreThreshold;
    }

    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
        var response = _fraudCheckAway.Check(new FraudAwayCheck
        {
            PersonFullName = $"{customerOrder.CustomerAddress.FirstName} {customerOrder.CustomerAddress.LastName}",
            PersonAddress = new PersonAddress
            {
                AddressLine1 = customerOrder.CustomerAddress.Line1,
                County = customerOrder.CustomerAddress.Region,
                Town = customerOrder.CustomerAddress.City,
                PostCode = customerOrder.CustomerAddress.PostalCode
            }
        });

        
        var fraudCheckStatus = FraudCheckStatus.Passed;
        if (response.FraudRiskScore > _riskScoreThreshold)
        {
            fraudCheckStatus = FraudCheckStatus.Failed;
        }
        
        _command.Execute(response, customerOrder);
            
        return new FraudCheckResponse
        {
            FraudCheckStatus = fraudCheckStatus, 
            CustomerGuid  = customerOrder.CustomerGuid,
            OrderId = orderId,
            OrderAmount = customerOrder.OrderAmount,
        };
    }
}