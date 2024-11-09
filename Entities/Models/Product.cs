using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; } // Unique SKU
        public DateTime CreatedDate { get; set; }

        // Navigation property to establish a one-to-many relationship
        //public ICollection<Inventory> Inventories { get; set; }
        //public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
    }
}
