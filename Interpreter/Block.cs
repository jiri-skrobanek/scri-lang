using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    class Block : IExecutable
    {
        List<Statement> statements;

        public ExecutionResult Execute(Scope s)
        {
            Scope myScope = new Scope() { ParentScope = s };
            foreach(Statement stat in statements)
            {
                stat.Execute(myScope);
            }
        }
    }
}
