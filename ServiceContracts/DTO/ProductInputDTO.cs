using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class ProductInputDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public DateTime CreatedDate { get; set; }
        // Use List<int> to store only category IDs
        public List<int> ProductCategories { get; set; }
    }
}
