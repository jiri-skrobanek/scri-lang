using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Environment
{
    class Builtin : Attribute
    {
        public string Name { get; protected set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    class BuiltinFunction : Builtin
    {        
        public BuiltinFunction(string Name)
        {
            this.Name = Name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    class BuiltinVariable : Builtin
    {
        public BuiltinVariable(string Name)
        {
            this.Name = Name;
        }
    }
}
