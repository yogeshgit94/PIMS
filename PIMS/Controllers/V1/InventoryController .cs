using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System.Security.Claims;

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

        //[HttpGet("low-inventory")]
        //public async Task<IActionResult> GetLowInventoryItems([FromQuery] int threshold)
        //{
        //    // Check if the user is an Admin
        //    var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        //    if (userRole != "Administrator")
        //    {
        //        return Unauthorized("You are not authorized to view this resource.");
        //    }

        //    var items = await _inventoryService.GetLowInventoryItemsAsync(threshold);
        //    return Ok(items);
        //}

        //[HttpPost("add-transaction")]
        //public async Task<IActionResult> AddInventoryTransaction(int productId, int quantity, string reason, int userId)
        //{

        //    // Check if the user is an Admin
        //    var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        //    if (userRole != "Administrator")
        //    {
        //        return Unauthorized("You are not authorized to perform this action.");
        //    }

        //    await _inventoryService.AddInventoryTransactionAsync(productId, quantity, reason, userId);
        //    return Ok("Inventory transaction added successfully");
        //}
    }
}
