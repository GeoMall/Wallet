using Microsoft.AspNetCore.Mvc;
using Moq;
using Wallet.Api.Controllers;
using Wallet.Models.Models;
using Wallet.Service.Services;

namespace Wallet.Client.Test;

public class WalletApiTests
{
    private readonly Mock<ICurrencyService> _currencyServiceMock;
    private readonly WalletsController _walletController;
    private readonly Mock<IWalletService> _walletServiceMock;

    public WalletApiTests()
    {
        _walletServiceMock = new Mock<IWalletService>();

        _currencyServiceMock = new Mock<ICurrencyService>();

        _walletController = new WalletsController(_walletServiceMock.Object, _currencyServiceMock.Object);
    }

    [Fact]
    public async Task WalletApiController_SuccessfullyCreateWallet()
    {
        var request = new WalletRequest { Currency = "EUR" };
        var expectedWalletResponse = new WalletCreateResponse { Id = Guid.NewGuid() };

        _walletServiceMock.Setup(s => s.CreateWallet(request))
            .ReturnsAsync(expectedWalletResponse);

        var result = await _walletController.CreateWallet(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<WalletCreateResponse>(okResult.Value);
        Assert.Equal(expectedWalletResponse.Id, response.Id);
    }

    [Fact]
    public async Task WalletApiController_CreateWalletBadRequest_OnException()
    {
        var request = new WalletRequest { Currency = "EUR" };

        _walletServiceMock.Setup(s => s.CreateWallet(request))
            .ThrowsAsync(new Exception("Couldn't create wallet"));

        var result = await _walletController.CreateWallet(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Couldn't create wallet", badRequest.Value);
    }

    [Fact]
    public async Task WalletApiController_GetWalletBalance_ConvertedBalance()
    {
        var walletId = Guid.NewGuid();
        var wallet = new WalletResponse { Id = walletId, Balance = 100, Currency = "EUR" };
        var currencyResponse = new CurrencyResponse { Currency = "USD", Rate = 1.2m };

        _walletServiceMock.Setup(s => s.GetWallet(walletId))
            .ReturnsAsync(wallet);
        _currencyServiceMock.Setup(s => s.GetCurrency("USD"))
            .ReturnsAsync(currencyResponse);
        _currencyServiceMock.Setup(s => s.ConvertAmount(100, "EUR", "USD", 1.2m))
            .Returns(120m);

        var result = await _walletController.GetWalletBalance(walletId, "USD");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var balance = Assert.IsType<decimal>(okResult.Value);
        Assert.Equal(120m, balance);
    }

    [Fact]
    public async Task WalletApiController_GetWalletBalanceReturnsBadRequest_OnException()
    {
        var walletId = Guid.NewGuid();
        _walletServiceMock.Setup(s => s.GetWallet(walletId))
            .ThrowsAsync(new Exception("Wallet not found"));

        var result = await _walletController.GetWalletBalance(walletId, "USD");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Wallet not found", badRequest.Value);
    }

    [Fact]
    public async Task WalletApiController_AdjustBalance_AddFundsSuccessfully()
    {
        var walletId = Guid.NewGuid();
        var updatedWallet = new AdjustBalanceWalletResponse { Id = walletId, Balance = 200 };

        _walletServiceMock.Setup(s => s.AdjustBalance(walletId, 100, "EUR", "AddFundsStrategy"))
            .ReturnsAsync(updatedWallet);

        var result = await _walletController.AdjustBalance(walletId, 100, "EUR", "AddFundsStrategy");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AdjustBalanceWalletResponse>(okResult.Value);
        Assert.Equal(200, response.Balance);
    }

    [Fact]
    public async Task WalletApiController_AdjustBalance_ReturnsBadRequest_WhenAmountNegative()
    {
        var result = await _walletController.AdjustBalance(Guid.NewGuid(), -10, "EUR", "AddFundsStrategy");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Amount cannot be negative", badRequest.Value);
    }

    [Fact]
    public async Task WalletApiController_AdjustBalance_ReturnsBadRequest_WhenStrategyEmpty()
    {
        var result = await _walletController.AdjustBalance(Guid.NewGuid(), 10, "EUR", "");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Strategy cannot be empty. It must be provided to adjust wallet balance", badRequest.Value);
    }

    [Fact]
    public async Task AdjustBalance_ReturnsBadRequest_OnException()
    {
        var walletId = Guid.NewGuid();
        _walletServiceMock.Setup(s => s.AdjustBalance(walletId, 50, "EUR", "AddFundsStrategy"))
            .ThrowsAsync(new Exception("An Error Occurred when adjusting balance"));

        var result = await _walletController.AdjustBalance(walletId, 50, "EUR", "AddFundsStrategy");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("An Error Occurred when adjusting balance", badRequest.Value);
    }
}