using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Product
    {
        public int ProductID { get; set; } // Auto-increment

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string SKU { get; set; } // Unique SKU

        public DateTime CreatedDate { get; set; }       
       public List<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();        
    }
}
