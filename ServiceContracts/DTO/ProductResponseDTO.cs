using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class ProductResponseDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CategoryDTO> Categories { get; set; }
    }

    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
