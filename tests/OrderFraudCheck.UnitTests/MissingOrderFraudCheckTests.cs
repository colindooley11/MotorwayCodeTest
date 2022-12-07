using MotorwayPaymentsCodeTest.Domain;
using MotorwayPaymentsCodeTest.Domain.Models;
using OrderFraudCheck.UnitTests.TestAdapters.Secondary;
using OrderFraudCheck.UnitTests.TestSetup;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace OrderFraudCheck.UnitTests;

public class MissingOrderFraudCheckTests : FraudTestsBase
{
    private OrderFraudCheckDetails? _orderFraudCheckDetails = null!;
    private Exception? _exceptionResult;

    [BddfyFact]
    public void RetrievingMissingOrderFraudCheck()
    {
        this.Given(s => s.An_Order_Fraud_Check_Doesnt_Exist())
            .When(s => s.The_Order_Fraud_Check_Is_Queried("ABC123"))
            .And(s=> s.A_Missing_Order_Fraud_Check_Exception_Is_Raised())
            .BDDfy();
    }

    private void A_Missing_Order_Fraud_Check_Exception_Is_Raised()
    {
        Assert.IsType<MissingOrderFraudCheckException>(_exceptionResult); 
        Assert.Equal($"The Order Fraud Check for Order Id: ABC123 can not be found", _exceptionResult!.Message);
    }

    private async Task The_Order_Fraud_Check_Is_Queried(string orderId)
    {
        var getOrderFraudCheckQuery = new GetOrderFraudCheckQueryTestAdapter(_orderFraudCheckDetails);
        var orderFraudCheckQuery = new OrderFraudCheckQuery(getOrderFraudCheckQuery);
        _exceptionResult =
                    await BddfyExceptionExtensions.ExecuteActionThatThrows(async () =>
                        await orderFraudCheckQuery.Get(orderId));
          
    }
 
    private void An_Order_Fraud_Check_Doesnt_Exist()
    {
        _orderFraudCheckDetails = null!;
    }
}