﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class LowInventoryAlertDTO
    {
        public int InventoryID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int ReorderThreshold { get; set; }
    }
}
