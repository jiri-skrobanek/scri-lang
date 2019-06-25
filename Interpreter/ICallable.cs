using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public delegate void Invocation(IList<IValue> Args, out IValue Result);

    interface ICallable
    {
        Invocation Call { get; }
    }
}
