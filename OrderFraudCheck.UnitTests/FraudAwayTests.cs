using Moq;
using MotorwayPaymentsCodeTest;
using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheck.UnitTests.TestAdapters;
using OrderFraudCheckCodeTest.UnitTests;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheck.UnitTests;

public class FraudAwayTests
{
    private CustomerOrder _customerOrder;
    private Mock<IFraudCheckAway> _fraudCheckAway;
    private FraudCheckResponse _result;
    private MotorwayPaymentsCodeTest.OrderFraudCheck _orderFraudCheck;
    private int _riskScoreThreshold;
    private Guid _customerGuid;
    private FraudProviderResponse _fraudProviderResponse;
    private Mock<ISaveFraudCheckDetailsCommand> _command;
    private SaveFraudCheckDetailsCommandAdapter _saveFraudCheckDetailsCommandAdapter;

    [BddfyFact]
    public void RequestSentToFraudAwayCorrectly()
    {
        this.Given(s => s.A_Customer_Order(new CustomerOrder
            {
                CustomerGuid =  Guid.Parse("57406e32-6a43-4dae-81d9-38bd7e349d54"),
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
            }))
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly())
            .BDDfy();
    }

    [BddfyFact]
    public void ReturnPassedResultFromFraudAway()
    {
        int riskScoreThreshold = 0;
        int riskScore = 0;

        _customerGuid = Guid.NewGuid();
        this.Given(s => s.A_Customer_Order(new CustomerOrder
            {
                CustomerGuid = _customerGuid,
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
            }))
            .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
            .And(s => s.Fraud_Away_Returns_Response(riskScore))
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .And(s => s.CustomerGuid_Is_Returned(_customerGuid))
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(_customerGuid))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .WithExamples(new ExampleTable("riskScoreThreshold", "riskScore")
            {
                { 2, 1 },
                { 10, 9.99999 },
                { 100, 99.99 }
            }).BDDfy();
    }

    private void Details_Of_The_Order_Are_Saved_To_The_Database()
    {
        Assert.Equal("10 High Street", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.Line1);
        Assert.Equal("John", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.FirstName);
        Assert.Equal("Doe", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.LastName);
        Assert.Equal("London", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.City);
        Assert.Equal("Greater London", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.Region);
        Assert.Equal("W1T 3HE", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.PostalCode);
    }

    private void Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(Guid customerGuid)
    {
        Assert.Equal("ABC1234", _saveFraudCheckDetailsCommandAdapter.Response.OrderId);
        Assert.Equal(customerGuid, _saveFraudCheckDetailsCommandAdapter.Response.CustomerGuid);
        Assert.Equal(1500.55M, _saveFraudCheckDetailsCommandAdapter.Response.OrderAmount);
        Assert.Equal(FraudCheckStatus.Passed, _saveFraudCheckDetailsCommandAdapter.Response.FraudCheckStatus);
    }

    private void Order_Amount_Is_Returned()
    {
        Assert.Equal(1500.55M, _result.OrderAmount);
    }

    private void Order_Id_Is_Returned()
    {
       Assert.Equal("ABC123", _result.OrderId);
    }

    private void CustomerGuid_Is_Returned(Guid expectedCustomerGuid)
    {
        Assert.Equal(expectedCustomerGuid, _result.CustomerGuid);
    }

    private void Fraud_Away_Returns_Response(decimal fraudRiskScore)
    {
        _fraudProviderResponse = new FraudProviderResponse
        {
            ResponseCode = 200,
            FraudRiskScore = fraudRiskScore
        };
    }

    private void The_Configured_Maximum_Acceptable_Risk_Score(int riskScoreThreshold)
    {
        _riskScoreThreshold = riskScoreThreshold;
    }

    private void A_Customer_Order(CustomerOrder customerOrder)
    {
        _customerOrder = customerOrder;
    }

    private void The_Fraud_Check_Is_Requested(string orderId)
    {
        _fraudCheckAway = new Mock<IFraudCheckAway>();
        _command = new Mock<ISaveFraudCheckDetailsCommand>(); 
        _fraudCheckAway.Setup(away => away.Check(It.IsAny<FraudAwayCheck>())).Returns(_fraudProviderResponse);
        _saveFraudCheckDetailsCommandAdapter = new SaveFraudCheckDetailsCommandAdapter();
        _orderFraudCheck = new MotorwayPaymentsCodeTest.OrderFraudCheck(_fraudCheckAway.Object, _saveFraudCheckDetailsCommandAdapter, _riskScoreThreshold);
        _result = _orderFraudCheck.Check(orderId, _customerOrder);
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