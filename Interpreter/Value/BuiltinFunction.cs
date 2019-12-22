using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Value
{

    public class BuiltinFunction : IValue, ICallable
    {
        public BuiltinFunction(Invocation invocation)
        {
            Call = invocation;
        }

        public ValueKind ValueKind { get { return ValueKind.Builtin; } }

        public Invocation Call { get; private set; }

        public bool GetTruthValue()
        {
            return true;
        }
    }
}
