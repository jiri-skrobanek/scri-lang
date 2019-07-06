using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public class Block : IExecutable
    {
        public IEnumerable<Statement> statements;

        public ExecutionResult Execute(Scope s)
        {
            Scope myScope = new Scope(s);
            foreach(Statement stat in statements)
            {
                var result = stat.Execute(myScope);
                switch (result.resultType)
                {
                    case ResultType.Break:
                    case ResultType.Continue:
                    case ResultType.Return:
                    case ResultType.Error:
                        return result;
                    case ResultType.Performed:
                        continue;
                }
            }
            return new PerformedResult();
        }
    }
}
