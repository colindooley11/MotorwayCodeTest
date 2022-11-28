using OrderFraudCheckCodeTest.UnitTests;

namespace MotorwayPaymentsCodeTest;

public class OrderFraudCheck
{
    private readonly IFraudCheckAway _fraudCheckAway;

    public OrderFraudCheck(IFraudCheckAway fraudCheckAway)
    {
        _fraudCheckAway = fraudCheckAway ?? throw new ArgumentNullException(nameof(fraudCheckAway));
    }

    public void Check(string abc123, CustomerOrder customerOrder)
    {
        _fraudCheckAway.Check(new FraudAwayCheck
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
    }
}