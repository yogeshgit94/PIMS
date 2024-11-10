using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{    
    public class Category
    {
        public int CategoryID { get; set; }

        [Required]
        public string Name { get; set; }
      
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
