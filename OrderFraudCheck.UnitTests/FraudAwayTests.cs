using Moq;
using MotorwayPaymentsCodeTest;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheckCodeTest.UnitTests;

public class FraudAwayTests
{
    private CustomerOrder _customerOrder;
    private Mock<IFraudCheckAway> _fraudCheckAway;
    private FraudCheckResponse _result;
    private OrderFraudCheck _orderFraudCheck;
    private string _riskScoreThreshold;
    private Guid _customerGuid;

    [BddfyFact]
    public void RequestSentToFraudAwayCorrectly()
    {
        this.Given(s => s.A_Customer_Order(Guid.NewGuid()))
            .When(s => s.The_Fraud_Check_Is_Requested())
            .Then(s => s.The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly())
            .BDDfy();
    }

    [BddfyFact]
    public void ReturnPassedResultFromFraudAway()
    {
        string riskScoreThreshold = null;
        string riskScore = null;

        _customerGuid = Guid.NewGuid();
        this.Given(s => s.A_Customer_Order(_customerGuid))
            .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
            .And(s => s.Fraud_Away_Returns_Response(riskScore))
            .When(s => s.The_Fraud_Check_Is_Requested())
            .And(s => s.CustomerGuid_Is_Returned(_customerGuid))
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.Details_Are_Saved_To_The_Database())
            .WithExamples(@"
                | RiskScoreThreshold    | RiskScore     | 
                | 2                     | 1             | 
                | 10                    | 9.99999       |
                | 100                   | 99.99         |

")
            .BDDfy();
    }

    private void Details_Are_Saved_To_The_Database()
    {
        //throw new NotImplementedException();
    }

    private void Order_Amount_Is_Returned()
    {
        //throw new NotImplementedException();
    }

    private void Order_Id_Is_Returned()
    {
        //throw new NotImplementedException();
    }

    private void CustomerGuid_Is_Returned(Guid expectedCustomerGuid)
    {
        Assert.Equal(expectedCustomerGuid, _result.CustomerGuid);
    }


    private void Fraud_Away_Returns_Response(string fraudRiskScore)
    {
        var fraudProviderResponse = new FraudProviderResponse
        {
            ResponseCode = 200,
            FraudRiskScore = Convert.ToDecimal(fraudRiskScore)
        };

        _fraudCheckAway.Setup(away => away.Check(It.IsAny<FraudAwayCheck>()))
            .Returns(fraudProviderResponse);
    }

    private void The_Configured_Maximum_Acceptable_Risk_Score(string riskScoreThreshold)
    {
        _riskScoreThreshold = riskScoreThreshold;
    }

    // Return Passed Result from FraudAway
    //
    //     Given a customer order
    //     And the configured maximum acceptable risk score is <RiskScoreThreshold>
    // When the order fraud check is requested
    //     And FraudAway returns a 200 response
    //     And FraudAway response has a FraudRiskScore of <FraudRiskScore>
    // Then the FraudCheckStatus in the response returned from the service is “Passed”
    // And the provided CustomerGuid is returned in the response
    // And the provided OrderId is returned in the response
    // And the provided OrderAmount is returned in the response
    // And the details of the customer order and FraudAway response is saved in the database
    //
    // Examples   RiskScoreThreshold FraudRiskScore
    // 2// 1//
    // 10// 9.99999
    // 100// 99.99


    private void A_Customer_Order(Guid customerGuid)
    {
        _customerOrder = new CustomerOrder
        {
            CustomerGuid = customerGuid,
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
        _orderFraudCheck = new OrderFraudCheck(_fraudCheckAway.Object, Convert.ToInt32(_riskScoreThreshold));
        _result = _orderFraudCheck.Check("ABC123", _customerOrder);
    }

    private void The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly()
    {
        _fraudCheckAway.Verify(away => away.Check(It.Is<FraudAwayCheck>(check => AssertFraudCheckAwayRequest(check))));
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