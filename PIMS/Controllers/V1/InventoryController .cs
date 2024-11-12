using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
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
        #region AddInventory

        /// <summary>
        /// Adds a new inventory.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///    /api/v{version}/Inventory/addInventory
        ///     {
        ///         "productID": 123,
        ///         "quantity": 50,
        ///         "reorderThreshold": 10,
        ///         "warehouseLocation": "RaipurCityCenter A1",
        ///         "userID": 1
        ///     }
        /// 
        /// Expected Input:
        /// - "productID": The unique identifier of the product. Must be a valid product ID in the system.
        /// - "quantity": The number of items to add to the inventory. Must be a positive integer.
        /// - "reorderThreshold": The minimum quantity at which the inventory item should be reordered. Must be a positive integer.
        /// - "warehouseLocation": The location within the warehouse where the item is stored, e.g., "RaipurCityCenter A1".
        /// - "userID": The unique identifier of the user performing this action.
        /// 
        /// Sample Responses:
        /// - **200 OK**: Inventory item added successfully.
        /// - **400 Bad Request**: Invalid input, or inventory item already exists for this product.
        /// - **500 Internal Server Error**: If an unexpected error occurs.
        /// 
        /// </remarks>
        /// <param name="addInventoryDto">The data transfer object containing the new inventory details.</param>
        /// <response code="200">If the inventory item is successfully added.</response>
        /// <response code="400">If an inventory item for this product already exists or if input data is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>               
        [HttpPost("addInventory")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddInventory([FromBody] AddInventoryDTO addInventoryDto)
        {
            await _inventoryService.AddInventoryAsync(addInventoryDto);
            return Ok("Inventory item added successfully.");
        }

        #endregion

        #region AddOrAdjustInventoryTransaction
        /// <summary>
        /// Adjusts an inventory transaction based on quantity changes (e.g., restocking or deductions).
        /// </summary>
        /// <remarks>
        /// This endpoint allows you to adjust the inventory quantity by specifying the transaction details such as inventory ID, quantity change, reason for the change, and the user performing the action.
        ///
        /// **Sample Request**:        
        /// {
        ///   "InventoryID": 1,
        ///   "quantityChange": 20,
        ///   "reason": "Restocking",
        ///   "userID": 1
        /// }        
        ///
        /// **Responses**:
        /// - `200 OK`: Inventory transaction adjusted successfully.
        /// - `400 Bad Request`: Invalid transaction details or inventory item not found.
        /// - `500 Internal Server Error`: An error occurred while adjusting the inventory transaction.             
        /// </remarks>


        [HttpPost("AdjustTransaction")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddOrAdjustInventoryTransaction([FromBody] InventoryTransactionDTO transactionDto)
        {
            await _inventoryService.AdjustInventoryTransactionAsync(transactionDto);
            return Ok("Inventory transaction adjusted successfully.");
        }
        #endregion

        #region GetLowInventoryAlerts
        /// <summary>
        /// Retrieves a list of inventory items that are below the reorder threshold (low inventory).
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves all inventory items that have a quantity below or equal to their reorder threshold, providing information like product name, quantity, and reorder threshold.
        ///
        /// **Sample Request**:        
        /// GET /api/v{version}/Inventory/lowInventoryAlerts        
        ///
        /// **Responses**:
        /// - `200 OK`: A list of low inventory items, including their product name, quantity, and reorder threshold.
        /// - `500 Internal Server Error'.       
        /// </remarks>


        [HttpGet("lowInventoryAlerts")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<List<LowInventoryAlertDTO>>> GetLowInventoryAlerts()
        {
            var alerts = await _inventoryService.GetLowInventoryAlertsAsync();
            return Ok(alerts);
        }
        #endregion

        #region AuditInventory
        /// <summary>
        /// Audits the inventory by adjusting the quantities and recording audit transactions.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to perform an inventory audit by specifying the new quantity of an inventory item, along with the reason for the change and the user performing the audit action.
        ///
        /// **Sample Request**:      
        /// {
        ///   "InventoryID": 1,
        ///   "NewQuantity": 100,
        ///   "reason": "Inventory Audit",
        ///   "userID": 1
        /// }        
        ///
        /// **Responses**:
        /// - `200 OK`: Inventory audited successfully..
        /// - `400 Bad Request`: Inventory item not found..
        /// - `500 Internal Server Error`.      
        /// </remarks>


        [HttpPost("auditInventory")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AuditInventory([FromBody] InventoryAuditDTO auditDto)
        {
            await _inventoryService.AuditInventoryAsync(auditDto);
            return Ok("Inventory audited successfully.");
        }
        #endregion
    }
}
