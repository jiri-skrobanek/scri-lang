using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    /// <summary>
    /// Represents a sequence of statements.
    /// </summary>
    public class Block : IExecutable
    {
        public IEnumerable<Statement> statements;

        public ExecutionResult Execute(Scope s)
        {
            Scope myScope = new Scope(s);
            return _execute(myScope);
        }

        protected ExecutionResult _execute(Scope scope)
        {
            foreach (Statement stat in statements)
            {
                var result = stat.Execute(scope);
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

        public ExecutionResult Execute(Environment env)
        {
            return _execute(env.GlobalScope);
        }
    }
}
