﻿using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;

namespace Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetLowInventoryItemsAsync(int threshold)
        {
            return await _context.Inventories
                                 .Where(i => i.Quantity <= threshold)
                                 .Include(i => i.Product)
                                 .ToListAsync();
        }

        public async Task AddInventoryTransactionAsync(int productId, int quantity, string reason, int userId)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductID == productId);
            if (inventory == null)
            {
                inventory = new Inventory { ProductID = productId, Quantity = quantity };
                _context.Inventories.Add(inventory);
            }
            else
            {
                inventory.Quantity += quantity;
            }

            inventory.LastUpdated = DateTime.Now;
            inventory.UpdateReason = reason;
            inventory.UserID = userId;

            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<Inventory>> GetInventoryByProductIdAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateInventoryAsync(int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        Task<List<Inventory>> IInventoryService.GetLowInventoryItemsAsync(int threshold)
        {
            throw new NotImplementedException();
        }
    }
}
