using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class InventoryAuditDTO
    {
        public int InventoryID { get; set; }
        public int NewQuantity { get; set; }
        public string Reason { get; set; }
        public string UserID { get; set; }
    }
}
