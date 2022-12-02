using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheckCodeTest.UnitTests;

namespace MotorwayPaymentsCodeTest;

public class OrderFraudCheck
{
    private readonly IFraudCheckAway _fraudCheckAway;
    private readonly ISaveFraudCheckDetailsCommand _command;
    private readonly int _riskScore;

    public OrderFraudCheck(IFraudCheckAway fraudCheckAway, ISaveFraudCheckDetailsCommand command, int riskScore)
    {
        _fraudCheckAway = fraudCheckAway ?? throw new ArgumentNullException(nameof(fraudCheckAway));
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _riskScore = riskScore;
    }

    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
        _ = _fraudCheckAway.Check(new FraudAwayCheck
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

        return new FraudCheckResponse
        {
            FraudCheckStatus = FraudCheckStatus.Passed, 
            CustomerGuid  = customerOrder.CustomerGuid,
            OrderId = orderId,
            OrderAmount = customerOrder.OrderAmount,
        };
    }
}