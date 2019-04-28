using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    interface ICallable
    {
        void Call(IEnumerable<Value> Args, out Value Result);
    }
}
