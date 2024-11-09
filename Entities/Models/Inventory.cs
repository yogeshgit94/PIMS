using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Inventory
    {
        public int InventoryID { get; set; }
        public int ProductID { get; set; } // Foreign key reference to Product
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string WarehouseLocation { get; set; }

        // To record transaction details
        public DateTime LastUpdated { get; set; }
        public string UpdateReason { get; set; }
        public int UserID { get; set; } // Reference to user who made the change

    }
}
