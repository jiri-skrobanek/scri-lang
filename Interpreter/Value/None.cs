using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Value
{
    public struct None : IValue
    {
        public ValueKind ValueKind { get { return ValueKind.None; } }

        public bool GetTruthValue()
        {
            return false;
        }

        public override string ToString()
        {
            return "None";
        }
    }
}
