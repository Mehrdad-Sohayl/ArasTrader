using ArasTrader.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArasTrader.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerSyncService _customerSyncService;

    public CustomersController(ICustomerSyncService customerSyncService)
    {
        _customerSyncService = customerSyncService;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncCustomers(CancellationToken cancellationToken)
    {
        var result = await _customerSyncService.SyncAsync(cancellationToken);

        if (result.IsSuccess)
            return Ok(result);

        if (result != null && result.Errors.Any())
            return BadRequest(result.Errors.FirstOrDefault()!.Message);

        return BadRequest();
    }
}
