using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public abstract class Statement : IExecutable
    {
        public abstract ExecutionResult Execute(Scope scope);
    }

    public class Assignment : Statement
    {
        String variable;
        IExpression expression;

        public override ExecutionResult Execute(Scope scope)
        {
            scope[variable] = expression.Evaluate(scope);
            return new PerformedResult();
        }
    }

    public class Conditional : Statement
    {
        IExpression condition;
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

    public class WhileLoop : Statement
    {
        public IExpression condition;
        public Block block;

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

    public class ReturnStatement : Statement
    {
        public IExpression expression;

        public override ExecutionResult Execute(Scope scope)
        {
            return new ReturnResult() { result = expression.Evaluate(scope) };
        }
    }

    public class BreakStatement : Statement
    {
        public override ExecutionResult Execute(Scope scope)
        {
            return new BreakResult();
        }
    }

    public class ContinueStatement : Statement
    {
        public override ExecutionResult Execute(Scope scope)
        {
            return new ContinueResult();
        }
    }

    public class PrintStatement : Statement
    {
        IExpression expression;

        public PrintStatement(IExpression expression)
        {
            this.expression = expression;
        }

        public override ExecutionResult Execute(Scope scope)
        {
            var value = expression.Evaluate(scope);

            Environment.PrintText(value.ToString());

            return new PerformedResult();
        }
    }

    public class CallStatement : Statement
    {
        FunctionCall Call { get; set; }

        public override ExecutionResult Execute(Scope scope)
        {
            Call.Evaluate(scope);
            return new PerformedResult();
        }
    }

    public class FunctionDefinition : Statement
    {
        List<String> Args;
        Block Body;
        String Name;

        public override ExecutionResult Execute(Scope scope)
        {
            scope[Name] = new Function(Args, Body, scope);
            return new PerformedResult();
        }
    }
}
