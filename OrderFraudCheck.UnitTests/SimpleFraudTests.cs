using Moq;
using MotorwayPaymentsCodeTest;
using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheck.UnitTests.TestAdapters;
using OrderFraudCheckCodeTest.UnitTests;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheck.UnitTests;

public class SimpleFraudTests
{
    private CustomerOrder _customerOrder;
    private Mock<IFraudCheckAway> _fraudCheckAway;
    private FraudCheckResponse _result;
    private MotorwayPaymentsCodeTest.OrderFraudCheck _orderFraudCheck;
    private decimal _riskScoreThreshold;
    private Guid _customerGuid;
    private FraudCheckAwayResponse _fraudCheckAwayResponse;
    private SaveOrderFraudCheckDetailsCommandAdapter _saveOrderFraudCheckDetailsCommandAdapter;
    private SimpleFraudCheckTestAdapter _simpleFraudCheckTestAdapter;


    [BddfyFact]
    public void  RequestSentToSimpleFraudCorrectly()
    {
        this.Given(s => s.A_Customer_Order())
            .And(s=> s.Fraud_Away_Returns_Response(0, 500))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Details_Of_The_Customer_Order_Are_Sent_To_SimpleFraud_Correctly())
            .BDDfy();
    }

    
    // [BddfyFact]
    // public void ReturnPassedResultFromFraudAway()
    // {
    //     int riskScoreThreshold = 0;
    //     int riskScore = 0;
    //
    //     this.Given(s => s.A_Customer_Order())
    //         .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
    //         .And(s => s.Fraud_Away_Returns_Response(riskScore, TODO))
    //         .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
    //         .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Passed))
    //         .And(s => s.CustomerGuid_Is_Returned())
    //         .And(s => s.Order_Id_Is_Returned())
    //         .And(s => s.Order_Amount_Is_Returned())
    //         .And(s => s.Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(riskScore))
    //         .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
    //         .WithExamples(new ExampleTable("riskScoreThreshold", "riskScore")
    //         {
    //             { 2,  1},
    //             { 10, 9.99999},
    //             {100, 99.99}
    //         }).BDDfy();
    // }

    // [BddfyFact]
    // public void ReturnFailedResultFromFraudAway()
    // {
    //     decimal riskScoreThreshold = 0;
    //     decimal riskScore = 0;
    //
    //     _customerGuid = Guid.NewGuid();
    //     this.Given(s => s.A_Customer_Order())
    //         .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
    //         .And(s => s.Fraud_Away_Returns_Response(riskScore, TODO))
    //         .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
    //         .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Failed))
    //         .And(s => s.CustomerGuid_Is_Returned())
    //         .And(s => s.Order_Id_Is_Returned())
    //         .And(s => s.Order_Amount_Is_Returned())
    //         .And(s => s.Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(riskScore))
    //         .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
    //         .WithExamples(new ExampleTable("riskScoreThreshold", "riskScore")
    //         {
    //             { 0.01,  0.02},
    //             { 99.99, 100}
    //         }).BDDfy();
    // }



    public void The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus expectedStatus)
    {
        Assert.Equal(expectedStatus, _result.FraudCheckStatus);
    }

    private void Details_Of_The_Order_Are_Saved_To_The_Database()
    {
        Assert.Equal("10 High Street", _saveOrderFraudCheckDetailsCommandAdapter.Order.CustomerAddress.Line1);
        Assert.Equal("John", _saveOrderFraudCheckDetailsCommandAdapter.Order.CustomerAddress.FirstName);
        Assert.Equal("Doe", _saveOrderFraudCheckDetailsCommandAdapter.Order.CustomerAddress.LastName);
        Assert.Equal("London", _saveOrderFraudCheckDetailsCommandAdapter.Order.CustomerAddress.City);
        Assert.Equal("Greater London", _saveOrderFraudCheckDetailsCommandAdapter.Order.CustomerAddress.Region);
        Assert.Equal("W1T 3HE", _saveOrderFraudCheckDetailsCommandAdapter.Order.CustomerAddress.PostalCode);
    }

    private void Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(decimal fraudRiskScore)
    {
        Assert.Equal(200, _saveOrderFraudCheckDetailsCommandAdapter.Response.ResponseCode);
        Assert.Equal(fraudRiskScore, _saveOrderFraudCheckDetailsCommandAdapter.Response.FraudRiskScore);
    }

    private void Order_Amount_Is_Returned()
    {
        Assert.Equal(1500.55M, _result.OrderAmount);
    }

    private void Order_Id_Is_Returned()
    {
       Assert.Equal("ABC123", _result.OrderId);
    }

    private void CustomerGuid_Is_Returned()
    {
        Assert.Equal(_customerGuid, _result.CustomerGuid);
    }

    private void Fraud_Away_Returns_Response(decimal fraudRiskScore, int response)
    {
        _fraudCheckAwayResponse = new FraudCheckAwayResponse
        {
            ResponseCode = response,
            FraudRiskScore = fraudRiskScore
        };
    }

    private void The_Configured_Maximum_Acceptable_Risk_Score(decimal riskScoreThreshold)
    {
        _riskScoreThreshold = riskScoreThreshold;
    }

    private void A_Customer_Order()
    {
        _customerOrder = new CustomerOrder
        {
            CustomerGuid = Guid.Parse("57406e32-6a43-4dae-81d9-38bd7e349d54"),
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

    private void The_Order_Fraud_Check_Is_Requested(string orderId)
    {
        _fraudCheckAway = new Mock<IFraudCheckAway>();
        _fraudCheckAway.Setup(away => away.Check(It.IsAny<FraudAwayCheck>())).Returns(_fraudCheckAwayResponse);
        _simpleFraudCheckTestAdapter = new SimpleFraudCheckTestAdapter(200);
        _saveOrderFraudCheckDetailsCommandAdapter = new SaveOrderFraudCheckDetailsCommandAdapter();
        _orderFraudCheck = new MotorwayPaymentsCodeTest.OrderFraudCheck(_fraudCheckAway.Object, _simpleFraudCheckTestAdapter, _saveOrderFraudCheckDetailsCommandAdapter, _riskScoreThreshold);
        _result = _orderFraudCheck.Check(orderId, _customerOrder);
    }

    private void The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly()
    {
        _fraudCheckAway.Verify(away => away.Check(It.Is<FraudAwayCheck>(check => AssertFraudCheckAwayRequest(check))));
    }

    private void The_Details_Of_The_Customer_Order_Are_Sent_To_SimpleFraud_Correctly()
    {
        Assert.Equal("John Doe", _simpleFraudCheckTestAdapter.SimpleFraudCheckDetails.Name);
        Assert.Equal("10 High Street", _simpleFraudCheckTestAdapter.SimpleFraudCheckDetails.AddressLine1);
        Assert.Equal("W1T 3HE", _simpleFraudCheckTestAdapter.SimpleFraudCheckDetails.PostCode);
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