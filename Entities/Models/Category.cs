using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }

        // Navigation property for many-to-many relationship
       // public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<ProductCategory> Products { get; set; } = new List<ProductCategory>();
    }
}
