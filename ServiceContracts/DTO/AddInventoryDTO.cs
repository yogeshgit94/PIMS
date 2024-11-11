using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class AddInventoryDTO
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int ReorderThreshold { get; set; }
        public string WarehouseLocation { get; set; }
        public int UserID { get; set; }
    }
}
