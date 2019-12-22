using Interpreter.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    interface IExecutable
    {
        ExecutionResult Execute(Scope s);
    }

    public enum ResultType
    {
        Performed, Return, Break, Continue, Error
    }

    /// <summary>
    /// Overlying class for results of statement execution.
    /// </summary>
    public abstract class ExecutionResult
    {
        public ResultType resultType;
    }

    class PerformedResult : ExecutionResult
    {
        
        public PerformedResult()
        {
            resultType = ResultType.Performed;
        }
    }

    class ReturnResult : ExecutionResult
    {
        public ReturnResult()
        {
            resultType = ResultType.Return;
        }

        public IValue result;
    }

    class BreakResult : ExecutionResult
    {
        public BreakResult()
        {
            resultType = ResultType.Break;
        }
    }

    class ContinueResult : ExecutionResult
    {
        public ContinueResult()
        {
            resultType = ResultType.Continue;
        }
    }
}
