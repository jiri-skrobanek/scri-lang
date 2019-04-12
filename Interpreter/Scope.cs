using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    class Scope
    {
        public bool Global = false;
        public Dictionary<String, INamable> Names = new Dictionary<string, INamable>();
        public Scope ParentScope;
    }
}
