using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions
{
    public class ProductIDNotFoundException : Exception
    {
        public ProductIDNotFoundException(string message) : base(message)
        {
        }



        public class CategoryIDNotFoundException : Exception
        {
            public CategoryIDNotFoundException(string message) : base(message) { }
        }               
    }
}
