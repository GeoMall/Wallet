using Microsoft.AspNetCore.Mvc;
using Wallet.Models.Models;
using Wallet.Service.Services;

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
        try
        {
            var wallet = await _walletService.CreateWallet(request);
            return Ok(wallet);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWalletBalance(Guid id, [FromQuery] string? currencyCode)
    {
        try
        {
            decimal walletBalance = 0;

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

            return Ok(walletBalance);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{id:guid}/adjustBalance")]
    public async Task<IActionResult> AdjustBalance(
        Guid id,
        [FromQuery] decimal amount,
        [FromQuery] string currency,
        [FromQuery] string strategy
    )
    {
        if (amount < 0)
            return BadRequest("Amount cannot be negative");

        if (string.IsNullOrEmpty(strategy))
            return BadRequest("Strategy cannot be empty. It must be provided to adjust wallet balance");

        try
        {
            var wallet = await _walletService.AdjustBalance(id, amount, currency, strategy);

            return Ok(wallet);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}