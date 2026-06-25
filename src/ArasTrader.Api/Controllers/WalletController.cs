using ArasTrader.Api.Contracts.Wallets;
using ArasTrader.Application.DTOs;
using ArasTrader.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace ArasTrader.Api.Controllers;

[ApiController]
[Route("api/wallet")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpPut]
    public async Task<IActionResult> Deposit(DepositWalletRequest request, CancellationToken cancellationToken)
    {
        var depositWalletRequest = new DepositWalletRequestDto(
            customerId: request.CustomerId,
            amount: request.Amount);

        var result = await _walletService.Deposit(depositWalletRequest, cancellationToken);

        if (result.IsSuccess)
            return Ok(result);

        if (result != null && result.Errors.Any())
            return BadRequest(result.Errors.FirstOrDefault()!.Message);

        return BadRequest();
    }

}

