﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Environment
{
    class Builtin : Attribute
    {
        public string Name { get; protected set; }
    }

    /// <summary>
    /// Provides a convenient way to call .NET methods from interpreter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class BuiltinFunction : Builtin
    {        
        public BuiltinFunction(string Name)
        {
            this.Name = Name;
        }
    }

    /// <summary>
    /// Provides a convenient way to pass variables to the interpreter, this binding is one-way.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    class BuiltinVariable : Builtin
    {
        public BuiltinVariable(string Name)
        {
            this.Name = Name;
        }
    }
}
