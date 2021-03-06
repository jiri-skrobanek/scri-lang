﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interpreter.Value;

namespace Interpreter
{
    /// <summary>
    /// Represents a language expression of different kinds.
    /// </summary>
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
        public string variableName;

        public IValue Evaluate(Scope scope)
        {
            return scope[variableName];
        }
    }

    public class FunctionCall : IExpression
    {
        public IExpression function;
        public byte ArgCount { get { return (byte)args.Count; } }
        public IList<IExpression> args;

        public IValue Evaluate(Scope scope)
        {
            var processed = from arg in args select arg.Evaluate(scope);
            var val = function.Evaluate(scope);
            if (val is ICallable ic)
            {
                ic.Call(processed.ToList(), out var result);
                return result;
            }
            else
            {
                return new None();
            }
        }
    }

    public class OperatorEvaluation : IExpression
    {
        public OperatorType @operator;
        public IExpression left_arg;
        public IExpression right_arg;

        public IValue Evaluate(Scope scope)
        {
            var processed_l = left_arg?.Evaluate(scope);
            var processed_r = right_arg?.Evaluate(scope);
            var application = Operator.GetApplication(@operator, processed_l?.ValueKind, processed_r?.ValueKind);
            return application(processed_l, processed_r);
        }
    }
}
