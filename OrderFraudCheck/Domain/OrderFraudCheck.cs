using MotorwayPaymentsCodeTest.Domain.Models;
using MotorwayPaymentsCodeTest.PrimaryPorts;
using MotorwayPaymentsCodeTest.SecondaryPorts;

namespace MotorwayPaymentsCodeTest.Domain;

public class OrderFraudCheck : IOrderFraudCheck
{
    private readonly IFraudCheckAway _fraudCheckAway;
    private readonly ISimpleFraudCheck _simpleFraudCheck;
    private readonly ISaveOrderFraudCheckDetailsCommand _saveOrderOrderFraudCheckDetailsCommand;
    private readonly ISaveOrderFraudCheckSimpleFraudDetailsCommand _saveOrderFraudCheckSimpleFraudDetailsCommand;
    private readonly decimal _riskScoreThreshold;

    public OrderFraudCheck(IFraudCheckAway fraudCheckAway, ISimpleFraudCheck simpleFraudCheck, ISaveOrderFraudCheckDetailsCommand saveOrderOrderFraudCheckDetailsCommand, ISaveOrderFraudCheckSimpleFraudDetailsCommand saveOrderFraudCheckSimpleFraudDetailsCommand, decimal riskScoreThreshold)
    {
        _fraudCheckAway = fraudCheckAway ?? throw new ArgumentNullException(nameof(fraudCheckAway));
        _simpleFraudCheck = simpleFraudCheck ?? throw new ArgumentNullException(nameof(simpleFraudCheck));
        _saveOrderOrderFraudCheckDetailsCommand = saveOrderOrderFraudCheckDetailsCommand ?? throw new ArgumentNullException(nameof(saveOrderOrderFraudCheckDetailsCommand));
        _saveOrderFraudCheckSimpleFraudDetailsCommand = saveOrderFraudCheckSimpleFraudDetailsCommand ?? throw new ArgumentNullException(nameof(saveOrderFraudCheckSimpleFraudDetailsCommand));
        _riskScoreThreshold = riskScoreThreshold;
    }


    public FraudCheckResponse Check(string orderId, CustomerOrder customerOrder)
    {
        var fraudCheckStatus = FraudCheckStatus.Failed;
        var fraudAwayChainedCheck = new FraudAwayChainedCheck(null, _fraudCheckAway,
            _saveOrderOrderFraudCheckDetailsCommand, _riskScoreThreshold);

        fraudCheckStatus =  fraudAwayChainedCheck.Check(orderId, customerOrder).FraudCheckStatus;
        if (fraudCheckStatus == FraudCheckStatus.DidNotComplete)
        {
            var simpleFraudChainedCheck = new SimpleFraudChainedCheck(null, _simpleFraudCheck,
                _saveOrderFraudCheckSimpleFraudDetailsCommand);

           fraudCheckStatus = simpleFraudChainedCheck.Check(orderId, customerOrder).FraudCheckStatus;

        }


        return new FraudCheckResponse
        {
            FraudCheckStatus = fraudCheckStatus, 
            CustomerGuid  = customerOrder.CustomerGuid,
            OrderId = orderId,
            OrderAmount = customerOrder.OrderAmount,
        };
    }

    private static string DeriveFullName(CustomerOrder customerOrder)
    {
        return $"{customerOrder.CustomerAddress.FirstName} {customerOrder.CustomerAddress.LastName}";
    }
}