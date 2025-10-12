namespace Wallet.Service.Services;

public class StrategyService
{
    public decimal AddFundsStrategy(decimal currentBalance, decimal amount)
    {
        return currentBalance + amount;
    }

    public decimal SubtractFundsStrategy(decimal currentBalance, decimal amount)
    {
        if (currentBalance < amount)
            throw new Exception("Could not perform action due to insufficient balance");

        return currentBalance - amount;
    }

    public decimal ForceSubtractFundsStrategy(decimal currentBalance, decimal amount)
    {
        return currentBalance - amount;
    }
}