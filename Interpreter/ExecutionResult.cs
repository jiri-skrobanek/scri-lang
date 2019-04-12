using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    abstract class ExecutionResult
    {

    }

    class PerformedResult : ExecutionResult
    {

    }

    class ReturnResult : ExecutionResult
    {
        public Value result;
    }

    class BreakResult : ExecutionResult
    {

    }

    class ContinueResult : ExecutionResult
    {

    }

    class ErrorResult : ExecutionResult
    {

    }
}
