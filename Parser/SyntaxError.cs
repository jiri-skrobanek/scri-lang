using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public class SyntaxError : Exception
    {
        public SyntaxError(string Message = null) : base(Message)
        {

        }
    }
}
