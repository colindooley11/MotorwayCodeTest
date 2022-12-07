using Moq;
using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.Domain.Services;
using MotorwayPaymentsCodeTest.SecondaryPorts;
using OrderFraudCheck.UnitTests.TestAdapters.Primary;
using OrderFraudCheck.UnitTests.TestAdapters.Secondary;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheck.UnitTests;

public class DefaultBypassTests
{
    private CustomerOrder _customerOrder;
    private FraudAwayProviderTestAdapter _fraudAwayProvider;
    private FraudCheckResponse _result;
    private MotorwayPaymentsCodeTest.Domain.OrderFraudCheck _orderFraudCheck;
    private decimal _riskScoreThreshold;
    private FraudAwayResult _fraudAwayResult;
    private SaveFraudAwayDetailsCommandTestAdapter _saveFraudAwayDetailsCommand;
    private ByPassThersholdDetailsCommandTestAdapter _byPassThersholdDetailsCommandTestAdapter;
    private Guid _customerId = Guid.Parse("57406e32-6a43-4dae-81d9-38bd7e349d54");
    private decimal _bypassAmountThreshold;
    private SimpleFraudResult _simpleFraudResult;
    private ISimpleFraudProvider _simpleFraudProvider;
    private decimal _savedOrderAmount;
    private decimal _savedBypassAmount;
    private DefaultFraudResult _defaultBypassResult;
    private OrderFraudCheckDetails _orderFraudCheckDetails;


    [BddfyFact]
    public void ReturnDefaultFraudBypassPassedResult()
    {
        decimal orderAmount = 0;
        decimal bypassAmountThreshold = 0;

        this.Given(s => s.A_Customer_Order(orderAmount))
            .And(s => s.The_Configured_Bypass_Amount(bypassAmountThreshold))
            .And(s => s.Fraud_Away_Returns_Response(500))
            .And(s => s.Simple_Fraud_Returns_Response(500))
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Passed))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned(orderAmount))
            .And(s => s.Details_Of_The_Bypass_Amount_Threshold_Used_Are_Saved_To_The_Database(bypassAmountThreshold))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .WithExamples(new ExampleTable("orderAmount", "bypassAmountThreshold")
            {
                { 999.99, 1000 },
                { 9999999.99, 10000000 },
                { 1, 1 }
            }).BDDfy();
    }
    
    [BddfyFact]
    public void ReturnDefaultFraudBypassFailedResult()
    {
        decimal orderAmount = 0;
        decimal bypassAmountThreshold = 0;

        this.Given(s => s.A_Customer_Order(orderAmount))
            .And(s => s.The_Configured_Bypass_Amount(bypassAmountThreshold))
            .And(s => s.Fraud_Away_Returns_Response(500))
            .And(s => s.Simple_Fraud_Returns_Response(500))
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .Then(s => s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Failed))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned(orderAmount))
            .And(s => s.Details_Of_The_Bypass_Amount_Threshold_Used_Are_Saved_To_The_Database(bypassAmountThreshold))
            .And(s => s.Details_Of_The_Order_Are_Saved_To_The_Database())
            .WithExamples(new ExampleTable("orderAmount", "bypassAmountThreshold")
            {
                { 0.01, 0 },
                { 10000000, 9999999 }
            }).BDDfy();
    }
    
    [BddfyFact]
    public void DuplicateDefaultFraudBypassPassedResult()
    {
        decimal orderAmount = 0;
        decimal bypassAmountThreshold = 0;

        this.Given(s => s.A_Bypassed_Order_Fraud_Check_Already_Exists(orderAmount,bypassAmountThreshold))
            .And(s => s.A_Customer_Order(orderAmount))
            .And(s=> s.The_Saved_Amounts(orderAmount, bypassAmountThreshold))
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .And(s=> s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Passed))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned(orderAmount))
            .WithExamples(new ExampleTable("orderAmount", "bypassAmountThreshold")
            {
                { 999.99, 1000 },
                { 9999999.99, 10000000 },
                { 1, 1 }
            })
            .And(s=> s.No_Remote_Calls_Are_Made())
            .And(s=> s.No_Details_Are_Saved_To_The_Database())
            .BDDfy();
    }
    
    [BddfyFact]
    public void DuplicateDefaultFraudBypassFailedResult()
    {
        decimal orderAmount = 0;
        decimal bypassAmountThreshold = 0;

        this.Given(s => s.A_Bypassed_Order_Fraud_Check_Already_Exists(orderAmount,bypassAmountThreshold))
            .And(s => s.A_Customer_Order(orderAmount))
            .And(s=> s.The_Saved_Amounts(orderAmount, bypassAmountThreshold))
            .When(s => s.The_Fraud_Check_Is_Requested("ABC123"))
            .And(s=> s.The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus.Failed))
            .And(s => s.CustomerGuid_Is_Returned())
            .And(s => s.Order_Id_Is_Returned())
            .And(s => s.Order_Amount_Is_Returned(orderAmount))
            .WithExamples(new ExampleTable("orderAmount", "bypassAmountThreshold")
            {
                { 0.01, 0 },
                { 10000000, 9999999 }
            })
            .And(s=> s.No_Remote_Calls_Are_Made())
            .And(s=> s.No_Details_Are_Saved_To_The_Database())
            .BDDfy();
    }

    private void No_Details_Are_Saved_To_The_Database()
    {
        Assert.Equal(null, _saveFraudAwayDetailsCommand.Response);
    }

    private void No_Remote_Calls_Are_Made()
    {
        Assert.Equal(null, _fraudAwayProvider.FraudAwayDetails);
    }
    
    private void The_Saved_Amounts(decimal orderAmount, decimal bypassAmountThreshold)
    {
        _orderFraudCheckDetails = new OrderFraudCheckDetails
        {
            DefaultFraudResult = new DefaultFraudResult { OrderAmount = orderAmount, BypassThresholdAmount = bypassAmountThreshold},
            CustomerOrder = TestData.DefaultCustomer(_customerId)
        };
    }

    private void A_Bypassed_Order_Fraud_Check_Already_Exists(decimal orderAmount, decimal bypassThresholdAmount)
    {
        _defaultBypassResult = new DefaultFraudResult { OrderAmount = orderAmount, BypassThresholdAmount = bypassThresholdAmount };
    }


    public void The_Fraud_Check_Status_Returned_From_The_Service_Is(FraudCheckStatus expectedStatus)
    {
        Assert.Equal(expectedStatus, _result.FraudCheckStatus);
    }

    private void Details_Of_The_Order_Are_Saved_To_The_Database()
    {
        Assert.Equal("10 High Street", _byPassThersholdDetailsCommandTestAdapter.Order.CustomerAddress.Line1);
        Assert.Equal("John", _byPassThersholdDetailsCommandTestAdapter.Order.CustomerAddress.FirstName);
        Assert.Equal("Doe", _byPassThersholdDetailsCommandTestAdapter.Order.CustomerAddress.LastName);
        Assert.Equal("London", _byPassThersholdDetailsCommandTestAdapter.Order.CustomerAddress.City);
        Assert.Equal("Greater London", _byPassThersholdDetailsCommandTestAdapter.Order.CustomerAddress.Region);
        Assert.Equal("W1T 3HE", _byPassThersholdDetailsCommandTestAdapter.Order.CustomerAddress.PostalCode);
    }

    private void Details_Of_The_Bypass_Amount_Threshold_Used_Are_Saved_To_The_Database(decimal bypassAmountThreshold)
    {
        Assert.Equal(bypassAmountThreshold, _byPassThersholdDetailsCommandTestAdapter.ByPassAmountThreshold);
    }

    private void Order_Amount_Is_Returned(decimal orderAmount)
    {
        Assert.Equal(orderAmount, _result.OrderAmount);
    }

    private void Order_Id_Is_Returned()
    {
        Assert.Equal("ABC123", _result.OrderId);
    }

    private void CustomerGuid_Is_Returned()
    {
        Assert.Equal(_customerId, _result.CustomerGuid);
    }

    private void Fraud_Away_Returns_Response(int responseCode)
    {
        _fraudAwayResult = new FraudAwayResult
        {
            ResponseCode = responseCode
        };
    }

    private void Simple_Fraud_Returns_Response(int responseCode)
    {
        _simpleFraudResult = new SimpleFraudResult
        {
            ResponseCode = responseCode
        };
    }

    private void The_Configured_Bypass_Amount(decimal bypassAmountThreshold)
    {
        _bypassAmountThreshold = bypassAmountThreshold;
    }

    private void A_Customer_Order(decimal orderAmount)
    {
        _customerOrder =  new CustomerOrder
        {
            CustomerGuid = _customerId,
            OrderAmount = orderAmount,
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
        _riskScoreThreshold = _riskScoreThreshold == 0 ? 100 : _riskScoreThreshold;

        _byPassThersholdDetailsCommandTestAdapter =  new ByPassThersholdDetailsCommandTestAdapter();
        var defaultFraudCheck = new DefaultFraudBypassFraudCheckService(_byPassThersholdDetailsCommandTestAdapter, _bypassAmountThreshold);

        _simpleFraudResult = _simpleFraudResult ?? new SimpleFraudResult() { ResponseCode = 500 };
        _simpleFraudProvider = new SimpleFraudProviderTestAdapter(_simpleFraudResult.Result, _simpleFraudResult.ResponseCode);
        var simpleFraudCheck = new SimpleFraudFraudCheckService(defaultFraudCheck, _simpleFraudProvider, Mock.Of<ISaveSimpleFraudDetailsCommand>());
    
        _fraudAwayProvider = new FraudAwayProviderTestAdapter(0, 500);
        _saveFraudAwayDetailsCommand = new SaveFraudAwayDetailsCommandTestAdapter();
        var fraudAwayFraudCheckService = new FraudAwayFraudCheckService(simpleFraudCheck, _fraudAwayProvider, _saveFraudAwayDetailsCommand, _riskScoreThreshold);

        var getOrderFraudCheckQuery = new GetOrderFraudCheckQueryTestAdapter(_orderFraudCheckDetails);
        var idempotentFraudCheckService = new IdempotentRemoteFraudCheckService(fraudAwayFraudCheckService, getOrderFraudCheckQuery);
        
        _orderFraudCheck = new MotorwayPaymentsCodeTest.Domain.OrderFraudCheck(idempotentFraudCheckService);
        _result = _orderFraudCheck.Check(orderId, _customerOrder);
    }
}