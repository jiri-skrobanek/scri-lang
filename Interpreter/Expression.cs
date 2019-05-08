using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
    public interface IExpression
    {
        Value Evaluate(Scope scope);
    }

    public class ConstantExpression : IExpression
    {
        public Value value;

        public Value Evaluate(Scope scope)
        {
            return value;
        }
    }

    public class VariableExpression : IExpression
    {
        public String variableName;

        public Value Evaluate(Scope scope)
        {
            return scope.FindVariable(variableName).value;
        }
    }

    public class FunctionCall : IExpression
    {
        public String functionName;
        public byte ArgCount { get { return (byte)args.Count; } }
        public List<IExpression> args;

        public Value Evaluate(Scope scope)
        {
            var processed = from arg in args select arg.Evaluate(scope);
            scope.FindFunction(functionName, ArgCount).Call(processed, out var result);
            return result;
        }
    }

    public class OperatorEvaluation : IExpression
    {
        public OperatorType @operator;
        public IExpression left_arg;
        public IExpression right_arg;

        public Value Evaluate(Scope scope)
        {
            var processed = from arg in args select arg.Evaluate(scope);
            @operator.Call(processed, out var result);
            return result;
        }
    }
}
