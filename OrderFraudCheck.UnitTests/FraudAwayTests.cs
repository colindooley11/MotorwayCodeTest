using Moq;
using MotorwayPaymentsCodeTest;
using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheck.UnitTests.TestAdapters;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheck.UnitTests;

public class FraudAwayTests
{
    private CustomerOrder _customerOrder;
    private Mock<IFraudCheckAway> _fraudCheckAway;
    private FraudCheckResponse _result;
    private MotorwayPaymentsCodeTest.Domain.OrderFraudCheck _orderFraudCheck;
    private decimal _riskScoreThreshold;
    private FraudCheckAwayResponse _fraudCheckAwayResponse;
    private Mock<ISaveOrderFraudCheckDetailsCommand> _command;
    private SaveOrderFraudCheckAwayDetailsCommandAdapter _saveOrderFraudCheckAwayDetailsCommandAdapter;
    private Guid _customerGuid = Guid.Parse("57406e32-6a43-4dae-81d9-38bd7e349d54");

    [BddfyFact]
    public void RequestSentToFraudAwayCorrectly()
    {
        this.Given(s => s.A_Customer_Order())
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly())
            .BDDfy();
    }

    [BddfyFact]
    public void ReturnPassedResultFromFraudAway()
    {
        decimal riskScoreThreshold = 0;
        decimal riskScore = 0;

        _customerGuid = Guid.NewGuid();
        this.Given(s => s.A_Customer_Order())
            .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
            .And(s => s.Fraud_Away_Returns_Response(riskScore, 200))
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Passed))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(riskScore))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .WithExamples(new ExampleTable("riskScoreThreshold", "riskScore")
            {
                { 2, 1 },
                { 10, 9.99999 },
                { 100, 99.99 }
            }).BDDfy();
    }

    [BddfyFact]
    public void ReturnFailedResultFromFraudAway()
    {
        decimal riskScoreThreshold = 0;
        decimal riskScore = 0;

        this.Given(s => s.A_Customer_Order())
            .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
            .And(s => s.Fraud_Away_Returns_Response(riskScore, 200))
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Failed))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(riskScore))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .WithExamples(new ExampleTable("riskScoreThreshold", "riskScore")
            {
                { 0.01, 0.02 },
                { 99.99, 100 }
            }).BDDfy();
    }


    public void The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus expectedStatus)
    {
        Assert.Equal(expectedStatus, _result.FraudCheckStatus);
    }

    private void Details_Of_The_Order_Are_Saved_To_The_Database()
    {
        Assert.Equal("10 High Street", _saveOrderFraudCheckAwayDetailsCommandAdapter.Order.CustomerAddress.Line1);
        Assert.Equal("John", _saveOrderFraudCheckAwayDetailsCommandAdapter.Order.CustomerAddress.FirstName);
        Assert.Equal("Doe", _saveOrderFraudCheckAwayDetailsCommandAdapter.Order.CustomerAddress.LastName);
        Assert.Equal("London", _saveOrderFraudCheckAwayDetailsCommandAdapter.Order.CustomerAddress.City);
        Assert.Equal("Greater London", _saveOrderFraudCheckAwayDetailsCommandAdapter.Order.CustomerAddress.Region);
        Assert.Equal("W1T 3HE", _saveOrderFraudCheckAwayDetailsCommandAdapter.Order.CustomerAddress.PostalCode);
    }

    private void Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(decimal fraudRiskScore)
    {
        Assert.Equal(200, _saveOrderFraudCheckAwayDetailsCommandAdapter.Response.ResponseCode);
        Assert.Equal(fraudRiskScore, _saveOrderFraudCheckAwayDetailsCommandAdapter.Response.FraudRiskScore);
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

    private void Fraud_Away_Returns_Response(decimal fraudRiskScore, int responseCode)
    {
        _fraudCheckAwayResponse = new FraudCheckAwayResponse
        {
            ResponseCode = responseCode,
            FraudRiskScore = fraudRiskScore
        };
    }

    private void The_Configured_Maximum_Acceptable_Risk_Score(decimal riskScoreThreshold)
    {
        _riskScoreThreshold = riskScoreThreshold;
    }

    private void A_Customer_Order()
    {
        _customerOrder = _customerOrder = new CustomerOrder
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
        };
    }

    private void The_Fraud_Check_Is_Requested(string orderId)
    {
        _fraudCheckAway = new Mock<IFraudCheckAway>();
        _command = new Mock<ISaveOrderFraudCheckDetailsCommand>();
        _riskScoreThreshold = _riskScoreThreshold == 0 ? 100 : _riskScoreThreshold;
        _fraudCheckAwayResponse = _fraudCheckAwayResponse ?? new FraudCheckAwayResponse { ResponseCode = 200, FraudRiskScore = 1};
        _fraudCheckAway.Setup(away => away.Check(It.IsAny<FraudAwayCheck>())).Returns(_fraudCheckAwayResponse);
        _saveOrderFraudCheckAwayDetailsCommandAdapter = new SaveOrderFraudCheckAwayDetailsCommandAdapter();
        _orderFraudCheck = new MotorwayPaymentsCodeTest.Domain.OrderFraudCheck(_fraudCheckAway.Object, Mock.Of<ISimpleFraudCheck>(), _saveOrderFraudCheckAwayDetailsCommandAdapter, Mock.Of<ISaveOrderFraudCheckSimpleFraudDetailsCommand>(), _riskScoreThreshold);
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