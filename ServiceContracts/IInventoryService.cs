using Entities.Models;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IInventoryService
    {
        Task AdjustInventoryTransactionAsync(InventoryTransactionDTO transactionDto);
        Task<List<LowInventoryAlertDTO>> GetLowInventoryAlertsAsync();
        Task AuditInventoryAsync(InventoryAuditDTO auditDto);

        // New method for adding inventory
        Task AddInventoryAsync(AddInventoryDTO addInventoryDto);
    }
}
