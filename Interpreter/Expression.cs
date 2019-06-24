using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
    public interface IExpression
    {
        IValue Evaluate(Scope scope);
    }

    public class ConstantExpression : IExpression
    {
        public IValue value;

        public IValue Evaluate(Scope scope)
        {
            return value;
        }
    }

    public class VariableExpression : IExpression
    {
        public String variableName;

        public IValue Evaluate(Scope scope)
        {
            return scope[variableName];
        }
    }

    public class FunctionCall : IExpression
    {
        public String functionName;
        public byte ArgCount { get { return (byte)args.Count; } }
        public List<IExpression> args;

        public IValue Evaluate(Scope scope)
        {
            var processed = from arg in args select arg.Evaluate(scope);
            (scope[functionName] as ICallable).Call(processed, out var result);
            return result;
        }
    }

    public class OperatorEvaluation : IExpression
    {
        public OperatorType @operator;
        public IExpression left_arg;
        public IExpression right_arg;

        public IValue Evaluate(Scope scope)
        {
            var processed_l = left_arg.Evaluate(scope);
            var processed_r = left_arg.Evaluate(scope);
            var op = Operator.
            @operator.Call(processed_l, processed_r, out var result);
            return result;
        }
    }
}
