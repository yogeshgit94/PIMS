﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions
{
    public class UnauthorizedAccessException : Exception
    {
        public UnauthorizedAccessException(string msg) : base(msg)
        {
        }
    }
}
