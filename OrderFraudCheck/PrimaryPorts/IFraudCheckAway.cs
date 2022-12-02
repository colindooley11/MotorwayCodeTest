namespace MotorwayPaymentsCodeTest;

public interface IFraudCheckAway
{
    public FraudCheckAwayResponse Check(FraudAwayCheck fraudAwayCheck);
}