using MotorwayPaymentsCodeTest.Domain;
using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.Domain.Services;
using OrderFraudCheck.UnitTests.TestAdapters.Primary;
using OrderFraudCheck.UnitTests.TestAdapters.Secondary;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheck.UnitTests;

public class FraudAwayTests : FraudTestsBase
{
    private FraudAwayProviderTestAdapter _fraudAwayProvider;
    private FraudCheckResponse _result;
    private MotorwayPaymentsCodeTest.Domain.OrderFraudCheck _orderFraudCheck;
    private decimal _riskScoreThreshold;
    private FraudAwayResult _fraudAwayResult;
    private SaveFraudAwayDetailsCommandTestAdapter _saveFraudAwayDetailsCommandTestAdapter;
    private Guid _customerId = Guid.Parse("57406e32-6a43-4dae-81d9-38bd7e349d54");
    private OrderFraudCheckDetails _orderFraudCheckDetails;

    [BddfyFact]
    public void RequestSentToFraudAwayCorrectly()
    {
        this.Given(s => s.A_Customer_Order(_customerId))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly())
            .BDDfy();
    }

    [BddfyFact]
    public void ReturnPassedResultFromFraudAway()
    {
        decimal riskScoreThreshold = 0;
        decimal riskScore = 0;

        this.Given(s => s.A_Customer_Order(_customerId))
            .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
            .And(s => s.Fraud_Away_Returns_Response(riskScore, 200))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
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

        this.Given(s => s.A_Customer_Order(_customerId))
            .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
            .And(s => s.Fraud_Away_Returns_Response(riskScore, 200))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
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


    [BddfyTheory]
    [InlineData(FraudCheckStatus.Passed)]
    [InlineData(FraudCheckStatus.Failed)]
    public void DuplicateOrderFraudAwayCheckRequest(FraudCheckStatus fraudCheckStatus)
    {
        this.Given(s => s.An_Order_Fraud_Check_From_Fraud_Away_Already_Exists(_customerId, fraudCheckStatus))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(fraudCheckStatus))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s=> s.No_Remote_Calls_Are_Made())
            .And(s=> s.No_Details_Are_Saved_To_The_Database())
            .BDDfy();
    }


    [BddfyTheory]
    [InlineData(FraudCheckStatus.Passed)]
    [InlineData(FraudCheckStatus.Failed)]
    public void RetrievingExistingFraudAwayCheck(FraudCheckStatus fraudCheckStatus)
    {
        this.Given(s => s.An_Order_Fraud_Check_From_Fraud_Away_Already_Exists(_customerId, fraudCheckStatus))
            .When(s => s.The_Order_Fraud_Check_Is_Queried("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(fraudCheckStatus))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .BDDfy();
    }

    private async Task The_Order_Fraud_Check_Is_Queried(string orderId)
    {
        var getOrderFraudCheckQuery = new GetOrderFraudCheckQueryTestAdapter(_orderFraudCheckDetails);
        var orderFraudCheckQuery = new OrderFraudCheckQuery(getOrderFraudCheckQuery);
        _result = await orderFraudCheckQuery.Get(orderId);
    }

    private void No_Details_Are_Saved_To_The_Database()
    {
        Assert.Null(_saveFraudAwayDetailsCommandTestAdapter.Response);
    }

    private void No_Remote_Calls_Are_Made()
    {
        Assert.Null(_fraudAwayProvider.FraudAwayDetails);
    }
    
    private void An_Order_Fraud_Check_From_Fraud_Away_Already_Exists(Guid customerGuid,
        FraudCheckStatus fraudCheckStatus)
    {
        _orderFraudCheckDetails = new OrderFraudCheckDetails
        {
            FraudCheckStatus = fraudCheckStatus,
            CustomerOrder = TestData.DefaultCustomer(customerGuid),
            FraudAwayResult = new FraudAwayResult
            {
                FraudRiskScore = 10,
                ResponseCode = 200
            }
        };
    }

    private void The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus expectedStatus)
    {
        Assert.Equal(expectedStatus, _result.FraudCheckStatus);
    }

    private void Details_Of_The_Order_Are_Saved_To_The_Database()
    {
        Assert.Equal("10 High Street", _saveFraudAwayDetailsCommandTestAdapter.Order.CustomerAddress.Line1);
        Assert.Equal("John", _saveFraudAwayDetailsCommandTestAdapter.Order.CustomerAddress.FirstName);
        Assert.Equal("Doe", _saveFraudAwayDetailsCommandTestAdapter.Order.CustomerAddress.LastName);
        Assert.Equal("London", _saveFraudAwayDetailsCommandTestAdapter.Order.CustomerAddress.City);
        Assert.Equal("Greater London", _saveFraudAwayDetailsCommandTestAdapter.Order.CustomerAddress.Region);
        Assert.Equal("W1T 3HE", _saveFraudAwayDetailsCommandTestAdapter.Order.CustomerAddress.PostalCode);
    }

    private void Details_Of_The_FraudAwayResponse_Are_Saved_To_The_Database(decimal fraudRiskScore)
    {
        Assert.Equal(200, _saveFraudAwayDetailsCommandTestAdapter.Response.ResponseCode);
        Assert.Equal(fraudRiskScore, _saveFraudAwayDetailsCommandTestAdapter.Response.FraudRiskScore);
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
        Assert.Equal(_customerId, _result.CustomerGuid);
    }

    private void Fraud_Away_Returns_Response(decimal fraudRiskScore, int responseCode)
    {
        _fraudAwayResult = new FraudAwayResult
        {
            ResponseCode = responseCode,
            FraudRiskScore = fraudRiskScore
        };
    }

    private void The_Configured_Maximum_Acceptable_Risk_Score(decimal riskScoreThreshold)
    {
        _riskScoreThreshold = riskScoreThreshold;
    }

    private async Task The_Order_Fraud_Check_Is_Requested(string orderId)
    {
        _riskScoreThreshold = _riskScoreThreshold == 0 ? 100 : _riskScoreThreshold;
        _fraudAwayResult = _fraudAwayResult ?? new FraudAwayResult { ResponseCode = 200, FraudRiskScore = 1 };
        _fraudAwayProvider = new FraudAwayProviderTestAdapter(_fraudAwayResult.FraudRiskScore, _fraudAwayResult.ResponseCode);
        _saveFraudAwayDetailsCommandTestAdapter = new SaveFraudAwayDetailsCommandTestAdapter();

        var fraudAwayFraudCheckService = new FraudAwayFraudCheckService(null!, _fraudAwayProvider,
            _saveFraudAwayDetailsCommandTestAdapter, _riskScoreThreshold);

        var getOrderFraudCheckQuery = new GetOrderFraudCheckQueryTestAdapter(_orderFraudCheckDetails);
        var idempotentFraudCheckService = new IdempotentRemoteFraudCheckService(fraudAwayFraudCheckService, getOrderFraudCheckQuery);
        
        _orderFraudCheck = new MotorwayPaymentsCodeTest.Domain.OrderFraudCheck(idempotentFraudCheckService);
        _result = await _orderFraudCheck.Check(orderId, _customerOrder);
    }

    private void The_Details_Of_The_Customer_Order_Are_Sent_To_FraudAway_Correctly()
    {
        Assert.Equal("John Doe", _fraudAwayProvider.FraudAwayDetails.PersonFullName);
        Assert.Equal("10 High Street", _fraudAwayProvider.FraudAwayDetails.PersonAddress.AddressLine1);
        Assert.Equal("London", _fraudAwayProvider.FraudAwayDetails.PersonAddress.Town);
        Assert.Equal("Greater London", _fraudAwayProvider.FraudAwayDetails.PersonAddress.County);
        Assert.Equal("W1T 3HE", _fraudAwayProvider.FraudAwayDetails.PersonAddress.PostCode);
    }
}