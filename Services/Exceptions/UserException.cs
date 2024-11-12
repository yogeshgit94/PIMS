using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions
{
    public class UserServiceException : Exception
    {
        public UserServiceException(string message) : base(message) { }

        public UserServiceException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UserAlreadyExistsException : UserServiceException
    {
        public UserAlreadyExistsException(string message) : base(message) { }
    }

    public class InvalidCredentialsException : UserServiceException
    {
        public InvalidCredentialsException(string message) : base(message) { }
    }

    public class UserNotFoundException : UserServiceException
    {
        public UserNotFoundException(string message) : base(message) { }
    }

    public class InvalidPasswordException : UserServiceException
    {
        public InvalidPasswordException(string message) : base(message) { }
    }
}
