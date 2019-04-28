using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public delegate Value OperatorApplication(Value Arg1, Value Arg2);

    public class Operator : ICallable
    {
        public byte Priority;
        public byte Arity;
        public ValueKind operand1;
        public ValueKind operand2;
        public OperatorApplication application;

        public Operator(byte Priority, byte Arity, ValueKind operand1, ValueKind operand2, OperatorApplication application)
        {
            this.Priority = Priority;
            this.Arity = Arity;
            this.operand1 = operand1;
            this.operand2 = operand2;
            this.application = application;
        }

        public void Call(IEnumerable<Value> Args, out Value Result)
        {
            var en = Args.GetEnumerator(); en.MoveNext();
            var first = en.Current;
            var second = en.MoveNext() ? en.Current : null;
            Result = application(first, second);
        }
    }

    public static class BuildinIntegralOperators
    {
        public static Operator plus = new Operator(40, 2, ValueKind.Integral, ValueKind.Integral, (x, y) => { return new IntegralValue((x as IntegralValue).value + (y as IntegralValue).value); } );
        public static Operator times = new Operator(50, 2, ValueKind.Integral, ValueKind.Integral, (x, y) => { return new IntegralValue((x as IntegralValue).value * (y as IntegralValue).value); });
        public static Operator minus = new Operator(40, 2, ValueKind.Integral, ValueKind.Integral, (x, y) => { return new IntegralValue((x as IntegralValue).value - (y as IntegralValue).value); });

    }
}
