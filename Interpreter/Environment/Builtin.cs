using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Environment
{
    class Builtin : Attribute
    {
    }

    class BuiltinFunction : Builtin
    {
        public string Name { get; }

        public BuiltinFunction(string Name)
        {
            this.Name = Name;
        }
    }

    class BuiltinVariable : Builtin
    {

    }
}
