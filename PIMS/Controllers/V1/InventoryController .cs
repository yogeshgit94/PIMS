using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace PIMS.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("low-inventory")]
        public async Task<IActionResult> GetLowInventoryItems([FromQuery] int threshold)
        {
            var items = await _inventoryService.GetLowInventoryItemsAsync(threshold);
            return Ok(items);
        }

        [HttpPost("add-transaction")]
        public async Task<IActionResult> AddInventoryTransaction(int productId, int quantity, string reason, int userId)
        {
            await _inventoryService.AddInventoryTransactionAsync(productId, quantity, reason, userId);
            return Ok("Inventory transaction added successfully");
        }
    }
}
