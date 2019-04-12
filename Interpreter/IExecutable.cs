using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    interface IExecutable
    {
        ExecutionResult Execute(Scope s);
    }
}
