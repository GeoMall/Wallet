using Wallet.Models.Models;
using Wallet.Service.Services;

namespace Wallet.Service.Factories;

public class StrategyFactory
{
    private readonly StrategyService _strategyService;

    public StrategyFactory(StrategyService strategyService)
    {
        _strategyService = strategyService;
    }

    public decimal Adjust(
        decimal amount,
        decimal currentBalance,
        string strategy
    )
    {
        return strategy switch
        {
            StrategyConst.AddFundsStrategy => _strategyService.AddFundsStrategy(currentBalance, amount),
            StrategyConst.SubtractFundsStrategy => _strategyService.SubtractFundsStrategy(currentBalance, amount),
            StrategyConst.ForceSubtractFundsStrategy => _strategyService.ForceSubtractFundsStrategy(currentBalance,
                amount),
            _ => throw new Exception("The provided strategy is not valid.")
        };
    }
}