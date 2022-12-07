using MotorwayPaymentsCodeTest.Domain;
using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.Domain.Services;
using OrderFraudCheck.UnitTests.TestAdapters.Primary;
using OrderFraudCheck.UnitTests.TestAdapters.Secondary;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheck.UnitTests;

public class SimpleFraudTests : FraudTestsBase
{
    private FraudCheckResponse _result;
    private MotorwayPaymentsCodeTest.Domain.OrderFraudCheck _orderFraudCheck;
    private decimal _riskScoreThreshold;
    private readonly Guid _customerId = Guid.Parse("57406e32-6a43-4dae-81d9-38bd7e349d54");
    private FraudAwayResult _fraudAwayResult;
    private SaveSimpleFraudDetailsCommandAdapter _saveSimpleFraudDetailsCommandAdapter;
    private SimpleFraudProviderTestAdapter _simpleFraudProviderTestAdapter;
    private OrderFraudCheckDetails _orderFraudCheckDetails;
    private FraudAwayProviderTestAdapter _fraudAwayProvider;
    private SaveFraudAwayDetailsCommandTestAdapter _saveFraudAwayDetailsCommandTestAdapter;


    [BddfyTheory]
    [InlineData(500)]
    [InlineData(503)]
    [InlineData(400)]
    public void RequestSentToSimpleFraudCorrectly(int responseCode)
    {
        this.Given(s => s.A_Customer_Order(Guid.Parse("57406e32-6a43-4dae-81d9-38bd7e349d54")))
            .And(s => s.Fraud_Away_Returns_Response(0, responseCode))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Details_Of_The_Customer_Order_Are_Sent_To_SimpleFraud_Correctly())
            .BDDfy();
    }

    [BddfyFact]
    public void ReturnPassedResultFromSimpleFraud()
    {
        int riskScoreThreshold = 0;
        int riskScore = 0;

        this.Given(s => s.A_Customer_Order(_customerId))
            .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
            .And(s => s.Fraud_Away_Returns_Response(riskScore, 500))
            .And(s => s.Simple_Fraud_Returns_Response("Pass", 200))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Passed))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.Details_Of_The_SimpleFraudResponse_Are_Saved_To_The_Database("Pass"))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .BDDfy();
    }

    [BddfyFact]
    public void ReturnFailedResultFromSimpleFraud()
    {
        int riskScoreThreshold = 0;
        int riskScore = 0;

        this.Given(s => s.A_Customer_Order(Guid.Parse("57406e32-6a43-4dae-81d9-38bd7e349d54")))
            .And(s => s.The_Configured_Maximum_Acceptable_Risk_Score(riskScoreThreshold))
            .And(s => s.Fraud_Away_Returns_Response(riskScore, 500))
            .And(_ => _.Simple_Fraud_Returns_Response("Fail", 200))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Failed))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.Details_Of_The_SimpleFraudResponse_Are_Saved_To_The_Database("Fail"))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .BDDfy();
    }

    [BddfyTheory]
    [InlineData(FraudCheckStatus.Failed)]
    [InlineData(FraudCheckStatus.Passed)]
    public void DuplicateOrderSimpleFraudCheckRequest(FraudCheckStatus fraudCheckStatus)
    {
        this.Given(s => s.An_Order_Fraud_Check_From_Simple_Fraud_Already_Exists(_customerId, fraudCheckStatus))
            .When(s => s.The_Order_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(fraudCheckStatus))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned())
            .And(s => s.No_Remote_Calls_Are_Made())
            .And(s => s.No_Details_Are_Saved_To_The_Database())
            .BDDfy();
    }
    
    [BddfyTheory]
    [InlineData(FraudCheckStatus.Passed)]
    [InlineData(FraudCheckStatus.Failed)]
    public void RetrievingExistingSimpleFraudCheck(FraudCheckStatus fraudCheckStatus)
    {
        this.Given(s => s.An_Order_Fraud_Check_From_Simple_Fraud_Already_Exists(_customerId, fraudCheckStatus))
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
        Assert.Null(_saveSimpleFraudDetailsCommandAdapter.Response);
        Assert.Null(_saveFraudAwayDetailsCommandTestAdapter.Response);
    }

    private void No_Remote_Calls_Are_Made()
    {
        Assert.Null(_simpleFraudProviderTestAdapter.SimpleFraudDetails);
        Assert.Null(_fraudAwayProvider.FraudAwayDetails);
    }

    private void An_Order_Fraud_Check_From_Simple_Fraud_Already_Exists(Guid customerId,
        FraudCheckStatus fraudCheckStatus)
    {
        _orderFraudCheckDetails = new OrderFraudCheckDetails
        {
            FraudCheckStatus = fraudCheckStatus,
            CustomerOrder = TestData.DefaultCustomer(customerId),
            SimpleFraudResult = new SimpleFraudResult
            {
                Result = "Passed",
                ResponseCode = 200
            }
        };
    }


    private void Simple_Fraud_Returns_Response(string result, int responseCode)
    {
        _simpleFraudProviderTestAdapter = new SimpleFraudProviderTestAdapter(result, responseCode);
    }

    private void The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus expectedStatus)
    {
        Assert.Equal(expectedStatus, _result.FraudCheckStatus);
    }

    private void Details_Of_The_Order_Are_Saved_To_The_Database()
    {
        Assert.Equal("10 High Street", _saveSimpleFraudDetailsCommandAdapter.Order.CustomerAddress.Line1);
        Assert.Equal("John", _saveSimpleFraudDetailsCommandAdapter.Order.CustomerAddress.FirstName);
        Assert.Equal("Doe", _saveSimpleFraudDetailsCommandAdapter.Order.CustomerAddress.LastName);
        Assert.Equal("London", _saveSimpleFraudDetailsCommandAdapter.Order.CustomerAddress.City);
        Assert.Equal("Greater London", _saveSimpleFraudDetailsCommandAdapter.Order.CustomerAddress.Region);
        Assert.Equal("W1T 3HE", _saveSimpleFraudDetailsCommandAdapter.Order.CustomerAddress.PostalCode);
    }

    private void Details_Of_The_SimpleFraudResponse_Are_Saved_To_The_Database(string expectedResult)
    {
        Assert.Equal(200, _saveSimpleFraudDetailsCommandAdapter.Response.ResponseCode);
        Assert.Equal(expectedResult, _saveSimpleFraudDetailsCommandAdapter.Response.Result);
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

    private void Fraud_Away_Returns_Response(decimal fraudRiskScore, int response)
    {
        _fraudAwayResult = new FraudAwayResult
        {
            ResponseCode = response,
            FraudRiskScore = fraudRiskScore
        };
    }

    private void The_Configured_Maximum_Acceptable_Risk_Score(decimal riskScoreThreshold)
    {
        _riskScoreThreshold = riskScoreThreshold;
    }

    private async Task The_Order_Fraud_Check_Is_Requested(string orderId)
    {
        _simpleFraudProviderTestAdapter ??= new SimpleFraudProviderTestAdapter("Passed", 200);
        _saveSimpleFraudDetailsCommandAdapter = new SaveSimpleFraudDetailsCommandAdapter();
        var simpleFraudFraudCheckService = new SimpleFraudFraudCheckService(null!, _simpleFraudProviderTestAdapter,
            _saveSimpleFraudDetailsCommandAdapter);

        _fraudAwayResult = _fraudAwayResult ?? new FraudAwayResult { ResponseCode = 200, FraudRiskScore = 1 };
        _fraudAwayProvider = new FraudAwayProviderTestAdapter(_fraudAwayResult.FraudRiskScore, _fraudAwayResult.ResponseCode);
        _saveFraudAwayDetailsCommandTestAdapter = new SaveFraudAwayDetailsCommandTestAdapter();
        var fraudAwayFraudCheckService = new FraudAwayFraudCheckService(simpleFraudFraudCheckService,
            _fraudAwayProvider,
            _saveFraudAwayDetailsCommandTestAdapter, _riskScoreThreshold);

        var getOrderFraudCheckQuery = new GetOrderFraudCheckQueryTestAdapter(_orderFraudCheckDetails);
        var idempotentFraudCheckService =
            new IdempotentRemoteFraudCheckService(fraudAwayFraudCheckService, getOrderFraudCheckQuery);

        _orderFraudCheck = new MotorwayPaymentsCodeTest.Domain.OrderFraudCheck(idempotentFraudCheckService);
        _result = await _orderFraudCheck.Check(orderId, _customerOrder);
    }

    private void The_Details_Of_The_Customer_Order_Are_Sent_To_SimpleFraud_Correctly()
    {
        Assert.Equal("John Doe", _simpleFraudProviderTestAdapter.SimpleFraudDetails.Name);
        Assert.Equal("10 High Street", _simpleFraudProviderTestAdapter.SimpleFraudDetails.AddressLine1);
        Assert.Equal("W1T 3HE", _simpleFraudProviderTestAdapter.SimpleFraudDetails.PostCode);
    }
}