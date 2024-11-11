using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class InventoryTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes it auto-increment
        public int InventoryTransactionID { get; set; }
        public int InventoryID { get; set; }
        public int QuantityChange { get; set; }
        public DateTime Timestamp { get; set; }
        public string Reason { get; set; }
        public string UserID { get; set; }  // User who made the change
        public Inventory Inventory { get; set; }
    }
}
