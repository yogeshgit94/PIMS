using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions
{
    public class InventoryAlreadyExistsException : Exception
    {
        public InventoryAlreadyExistsException(string message) : base(message)
        {
        }



        public class ProductIDDoestNotExitException : Exception
        {
            public ProductIDDoestNotExitException(string message) : base(message) { }
        }        

        public class InventoryNotFoundException : Exception
        {
            public InventoryNotFoundException(string message) : base(message) { }
        }

        public class InvalidTransactionException : Exception
        {
            public InvalidTransactionException(string message) : base(message) { }
        }
    }
}
