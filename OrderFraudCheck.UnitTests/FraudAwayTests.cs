using Moq;
using MotorwayPaymentsCodeTest;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheckCodeTest.UnitTests;

public class FraudAwayTests
{
    private CustomerOrder _customerOrder;
    private Mock<IFraudCheckAway> _fraudCheckAway;

    [BddfyFact]
    public void RequestSentToFraudAwayCorrectly()
    {
        this.Given(s => s.A_Customer_Order())
            .When(s => s.The_Fraud_Check_Is_Requested())
            .Then(s => s.The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly())
            .BDDfy();
    }

    private void A_Customer_Order()
    {
        _customerOrder = new CustomerOrder
        {
            CustomerGuid = Guid.NewGuid(),
            OrderAmount = 1500.55M,
            CustomerAddress = new Address
            {
                FirstName = "John",
                LastName = "Doe",
                Line1 = "10 High Street",
                City = "London",
                Region = "Greater London",
                PostalCode = "W1T 3HE"
            }
        };
    }

    private void The_Fraud_Check_Is_Requested()
    {
        _fraudCheckAway = new Mock<IFraudCheckAway>();
        var orderFraudCheck = new OrderFraudCheck(_fraudCheckAway.Object); 
        orderFraudCheck.Check("ABC123", _customerOrder);
    }

    private void The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly()
    {
        _fraudCheckAway.Verify(away => away.Check(It.Is<FraudAwayCheck>(check => AssertFraudCheckAwayRequest(check) )));
    }

    private bool AssertFraudCheckAwayRequest(FraudAwayCheck check)
    {
        Assert.Equal(check.PersonFullName, "John Doe");
        Assert.Equal(check.PersonAddress.AddressLine1, "10 High Street");
        Assert.Equal(check.PersonAddress.Town, "London");
        Assert.Equal(check.PersonAddress.County, "Greater London");
        Assert.Equal(check.PersonAddress.PostCode, "W1T 3HE");
        return true;
    }
}