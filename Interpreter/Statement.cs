using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    abstract class Statement : IExecutable
    {
        public abstract ExecutionResult Execute(Scope scope);
    }

    class Assignment : Statement
    {
        Variable variable;
        Expression expression;

        public override ExecutionResult Execute(Scope scope)
        {
            variable.value = expression.Evaluate(scope);
            return new PerformedResult();
        }
    }

    class Conditional : Statement
    {
        Expression condition;
        Block satisfied;
        Block unsatisfied;

        public override ExecutionResult Execute(Scope scope)
        {
            if(condition.Evaluate(scope).GetTruthValue())
            {
                return satisfied.Execute(scope);
            }
            else
            {
                return unsatisfied.Execute(scope);
            }
        }
    }

    class WhileLoop : Statement
    {
        Expression condition;
        Block block;

        public override ExecutionResult Execute(Scope scope)
        {
            while(condition.Evaluate(scope).GetTruthValue())
            {
                var result = block.Execute(scope);

                if (result.GetType() == typeof(BreakResult)) break;
                else if (result.GetType() == typeof(ContinueResult)) continue;
                else if (result.GetType() == typeof(ReturnResult)) return result;
                else if (result.GetType() == typeof(ErrorResult)) return result;
            }
            return new PerformedResult();
        }
    }

    class ReturnStatement : Statement
    {
        Expression expression;

        public override ExecutionResult Execute(Scope scope)
        {
            return new ReturnResult() { result = expression.Evaluate(scope) };
        }
    }

    class BreakStatement : Statement
    {
        public override ExecutionResult Execute(Scope scope)
        {
            return new BreakResult();
        }
    }

    class ContinueStatement : Statement
    {
        public override ExecutionResult Execute(Scope scope)
        {
            return new ContinueResult();
        }
    }

    class CallStatement : Statement
    {
        public override ExecutionResult Execute(Scope scope)
        {
            throw new NotImplementedException();
        }
    }
}
