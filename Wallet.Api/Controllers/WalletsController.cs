using Microsoft.AspNetCore.Mvc;
using Wallet.Models.Models;
using Wallet.Service;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly CurrencyService _currencyService;
    private readonly WalletService _walletService;

    public WalletsController(WalletService walletService, CurrencyService currencyService)
    {
        _walletService = walletService;
        _currencyService = currencyService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] WalletRequest request)
    {
        var wallet = await _walletService.CreateWallet(request);

        return Ok(wallet);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWalletBalance(Guid id, [FromQuery] string? currencyCode)
    {
        decimal walletBalance = 0;

        try
        {
            var wallet = await _walletService.GetWallet(id);

            if (currencyCode != null)
            {
                var currency = await _currencyService.GetCurrency(currencyCode);
                walletBalance = _currencyService.ConvertAmount(
                    wallet.Balance,
                    wallet.Currency,
                    currencyCode,
                    currency.Rate
                );
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok(walletBalance);
    }

    [HttpPost("{id:guid}/deposit")]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] decimal amount)
    {
        var wallet = await _walletService.Deposit(id, amount);

        return Ok(wallet);
    }

    [HttpPost("{id:guid}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] decimal amount)
    {
        var wallet = await _walletService.Withdraw(id, amount);

        return Ok(wallet);
    }
}