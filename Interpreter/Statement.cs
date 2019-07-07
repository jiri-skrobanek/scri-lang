using System.Collections.Generic;

namespace Interpreter
{
    /// <summary>
    /// Base class for representing statements.
    /// </summary>
    public abstract class Statement : IExecutable
    {
        public abstract ExecutionResult Execute(Scope scope);
    }

    public class AssignmentStatement : Statement
    {
        public string Name { get; set; }
        public IExpression Expression { get; set; }

        public override ExecutionResult Execute(Scope scope)
        {
            scope[Name] = Expression.Evaluate(scope);
            return new PerformedResult();
        }
    }

    public class ConditionalStatement : Statement
    {
        public IExpression condition;
        public Block satisfied;
        public Block unsatisfied;

        public override ExecutionResult Execute(Scope scope)
        {
            if (condition.Evaluate(scope).GetTruthValue())
            {
                return satisfied.Execute(scope);
            }
            else if(unsatisfied != null)
            {
                return unsatisfied.Execute(scope);
            }
            else
            {
                return new PerformedResult();
            }
        }
    }

    public class WhileLoop : Statement
    {
        public IExpression condition;
        public Block block;

        public override ExecutionResult Execute(Scope scope)
        {
            while (condition.Evaluate(scope).GetTruthValue())
            {
                var result = block.Execute(scope);

                if (result.GetType() == typeof(BreakResult))
                {
                    break;
                }
                else if (result.GetType() == typeof(ContinueResult))
                {
                    continue;
                }
                else if (result.GetType() == typeof(ReturnResult))
                {
                    return result;
                }
            }
            return new PerformedResult();
        }
    }

    public class ReturnStatement : Statement
    {
        public IExpression Expression { get; set; }

        public override ExecutionResult Execute(Scope scope)
        {
            return new ReturnResult() { result = Expression.Evaluate(scope) };
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
        private IExpression expression;

        public PrintStatement(IExpression expression)
        {
            this.expression = expression;
        }

        public override ExecutionResult Execute(Scope scope)
        {
            var value = expression.Evaluate(scope);

            scope.Environment.PrintText(value.ToString());

            return new PerformedResult();
        }
    }

    public class CallStatement : Statement
    {
        public FunctionCall Call { get; set; }

        public override ExecutionResult Execute(Scope scope)
        {
            Call.Evaluate(scope);
            return new PerformedResult();
        }
    }

    public class FunctionDefinition : Statement
    {
        public IList<string> Args;
        public Block Body;
        public string Name;

        public override ExecutionResult Execute(Scope scope)
        {
            scope[Name] = new Function(Args, Body, scope);
            return new PerformedResult();
        }
    }
}
