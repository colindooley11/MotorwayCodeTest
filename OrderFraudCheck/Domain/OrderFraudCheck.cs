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
        var fraudCheckAwayResponse = _fraudCheckAway.Check(new FraudAwayCheck
        {
            PersonFullName = DeriveFullName(customerOrder),
            PersonAddress = new PersonAddress
            {
                AddressLine1 = customerOrder.CustomerAddress.Line1,
                County = customerOrder.CustomerAddress.Region,
                Town = customerOrder.CustomerAddress.City,
                PostCode = customerOrder.CustomerAddress.PostalCode
            }
        });

        if (fraudCheckAwayResponse.ResponseCode == 200)
        {
            if (fraudCheckAwayResponse.FraudRiskScore < _riskScoreThreshold)
            {
                fraudCheckStatus = FraudCheckStatus.Passed;
            }

            _saveOrderOrderFraudCheckDetailsCommand.Execute(fraudCheckAwayResponse, customerOrder);
        }

        if (fraudCheckAwayResponse.ResponseCode != 200)
        {
            var simpleFraudCheckResponse = _simpleFraudCheck.Check(new SimpleFraudCheck
            {
                AddressLine1 = customerOrder.CustomerAddress.Line1,
                Name = DeriveFullName(customerOrder),
                PostCode = customerOrder.CustomerAddress.PostalCode
            });

            if (simpleFraudCheckResponse.ResponseCode == 200)
            {
                fraudCheckStatus = simpleFraudCheckResponse.Result == "Pass"
                    ? FraudCheckStatus.Passed
                    : FraudCheckStatus.Failed;
            }
            
            _saveOrderFraudCheckSimpleFraudDetailsCommand.Execute(simpleFraudCheckResponse, customerOrder);

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