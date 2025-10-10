using Microsoft.AspNetCore.Mvc;
using Wallet.Service;
using Wallet.Service.Models;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly WalletService _walletService;

    public WalletsController(WalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] WalletRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId))
            return BadRequest("UserId is required");

        var wallet = await _walletService.CreateWallet(request);

        return Ok(wallet);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWallet(Guid id)
    {
        WalletResponse wallet;

        try
        {
            wallet = await _walletService.GetWallet(id);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok(wallet);
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