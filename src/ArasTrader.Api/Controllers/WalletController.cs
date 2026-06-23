using ArasTrader.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace ArasTrader.Api.Controllers;

[ApiController]
[Route("/api/wallet")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpPut]
    public async Task<IActionResult> Deposit(Contracts.Wallets.DepositWalletRequest request, CancellationToken cancellationToken)
    {
        var depositWalletRequest = new Application.Models.Wallets.DepositWalletRequest(
            customerId: request.CustomerId,
            amount: request.Amount);

        await _walletService.Deposit(depositWalletRequest, cancellationToken);
        return Ok(new
        {
            message = "Wallet deposit successfully."
        });
    }

}

