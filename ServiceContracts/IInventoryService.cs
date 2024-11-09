using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetInventoryByProductIdAsync(int productId);
        Task AddInventoryTransactionAsync(int productId, int quantity, string reason, int userId);
        Task UpdateInventoryAsync(int productId, int quantity);
        Task<List<Inventory>> GetLowInventoryItemsAsync(int threshold);
    }
}
