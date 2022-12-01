namespace MotorwayPaymentsCodeTest;

public interface IFraudCheckAway
{
    public FraudProviderResponse Check(FraudAwayCheck fraudAwayCheck);
}