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
        await _customerSyncService.SyncAsync(cancellationToken);
        return Ok(new
        {
            message = "Customer sync completed."
        });
    }
}
