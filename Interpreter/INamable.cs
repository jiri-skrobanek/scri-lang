using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    interface INamable
    {
        String Name { get; set; }
    }

    public class NameCollection
    {
        public Variable variable = null;
        public Function[] functions = new Function[256];
    }
}
