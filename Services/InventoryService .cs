using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Exceptions;
using static Services.Exceptions.InventoryAlreadyExistsException;

namespace Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method for adding a new inventory item
        public async Task AddInventoryAsync(AddInventoryDTO addInventoryDto)
        {
            // Check if an inventory item for the given product already exists
            var existingInventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductID == addInventoryDto.ProductID);

            if (existingInventory != null)
            {
                throw new InventoryAlreadyExistsException("Inventory already exists for this product.");
            }

            var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.ProductID == addInventoryDto.ProductID);
            if (product == null)
            {
                throw new ProductIDDoestNotExitException($"Product with ID {addInventoryDto.ProductID} does not exist.");
            }




            // Create new inventory item
            var newInventory = new Inventory
            {
                ProductID = addInventoryDto.ProductID,
                Quantity = addInventoryDto.Quantity,
                ReorderThreshold = addInventoryDto.ReorderThreshold,
                WarehouseLocation = addInventoryDto.WarehouseLocation,
                UserID = addInventoryDto.UserID
            };

            // Add the new inventory item to the database
            try
            {
                _context.Inventories.Add(newInventory);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                string message = ex.InnerException.Message;
            }
        }

        //Adjust Inventory Transaction
        public async Task AdjustInventoryTransactionAsync(InventoryTransactionDTO transactionDto)
        {
            var inventory = await _context.Inventories.FindAsync(transactionDto.InventoryID);
            if (inventory == null) throw new InventoryNotFoundException("Inventory item not found.");

            // Adjust inventory quantity
            inventory.Quantity += transactionDto.QuantityChange;

            // Create new transaction
            var transaction = new InventoryTransaction
            {
                InventoryID = transactionDto.InventoryID,
                QuantityChange = transactionDto.QuantityChange,
                Timestamp = DateTime.Now,
                Reason = transactionDto.Reason,
                UserID = transactionDto.UserID
            };

            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        // Get Low Inventory Alerts
        public async Task<List<LowInventoryAlertDTO>> GetLowInventoryAlertsAsync()
        {
            return await _context.Inventories
                .Where(inv => inv.Quantity <= inv.ReorderThreshold)
                .Select(inv => new LowInventoryAlertDTO
                {
                    InventoryID = inv.InventoryID,
                    ProductName = inv.Product.Name,
                    Quantity = inv.Quantity,
                    ReorderThreshold = inv.ReorderThreshold
                })
                .ToListAsync();
        }

        //Audit Inventory
        public async Task AuditInventoryAsync(InventoryAuditDTO auditDto)
        {
            var inventory = await _context.Inventories.FindAsync(auditDto.InventoryID);
            if (inventory == null) throw new InventoryNotFoundException("Inventory item not found.");

            // Calculate quantity change for audit transaction
            int quantityChange = auditDto.NewQuantity - inventory.Quantity;
            // Update inventory quantity
            inventory.Quantity = auditDto.NewQuantity;
            // Create audit transaction
            var auditTransaction = new InventoryTransaction
            {
                InventoryID = auditDto.InventoryID,
                QuantityChange = quantityChange,
                Timestamp = DateTime.Now,
                Reason = auditDto.Reason,
                UserID = auditDto.UserID
            };
            _context.InventoryTransactions.Add(auditTransaction);
            await _context.SaveChangesAsync();
        }
    }
}
