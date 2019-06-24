using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public enum ResultType
    {
        Performed, Return, Break, Continue, Error
    }

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

    class ErrorResult : ExecutionResult
    {
        public string Message;

        public ErrorResult()
        {
            resultType = ResultType.Error;
        }
    }
}
