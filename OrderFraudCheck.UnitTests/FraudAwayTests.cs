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
    private decimal _riskScoreThreshold;
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
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Passed))
            .And(s => s.CustomerGuid_Is_Returned(_customerGuid))
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(riskScore))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .WithExamples(new ExampleTable("riskScoreThreshold", "riskScore")
            {
                { 2,  1},
                { 10, 9.99999},
                {100, 99.99}
            }).BDDfy();
    }

    [BddfyFact]
    public void ReturnFailedResultFromFraudAway()
    {
        decimal riskScoreThreshold = 0;
        decimal riskScore = 0;

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
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Failed))
            .And(s => s.CustomerGuid_Is_Returned(_customerGuid))
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(riskScore))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .WithExamples(new ExampleTable("riskScoreThreshold", "riskScore")
            {
                { 0.01,  0.02},
                { 99.99, 100}
            }).BDDfy();
    }
    
    
    public void The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus expectedStatus)
    {
        Assert.Equal(expectedStatus,_result.FraudCheckStatus);
    }

    
    // Return Failed Result from FraudAway

    //     Given a customer order
    //     And the configured maximum acceptable risk score is <RiskScoreThreshold>
    // When the order fraud check is requested
    //     And FraudAway returns a 200 response
    //     And FraudAway response has a FraudRiskScore of <FraudRiskScore>
    // Then the FraudCheckStatus in the response returned from the service is “Failed”
    // And the provided CustomerGuid is returned in the response
    // And the provided OrderId is returned in the response
    // And the provided OrderAmount is returned in the response
    // And the details of the customer order and FraudAway response is saved in the database
    //
    // Examples
    //     RiskScoreThreshold	FraudRiskScore
    // 0.01	0.02
    // 99.99	100


    private void Details_Of_The_Order_Are_Saved_To_The_Database()
    {
        Assert.Equal("10 High Street", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.Line1);
        Assert.Equal("John", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.FirstName);
        Assert.Equal("Doe", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.LastName);
        Assert.Equal("London", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.City);
        Assert.Equal("Greater London", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.Region);
        Assert.Equal("W1T 3HE", _saveFraudCheckDetailsCommandAdapter.Order.CustomerAddress.PostalCode);
    }

    private void Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(decimal fraudRiskScore)
    {
        Assert.Equal(200, _saveFraudCheckDetailsCommandAdapter.Response.ResponseCode);
        Assert.Equal(fraudRiskScore, _saveFraudCheckDetailsCommandAdapter.Response.FraudRiskScore);
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

    private void The_Configured_Maximum_Acceptable_Risk_Score(decimal riskScoreThreshold)
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