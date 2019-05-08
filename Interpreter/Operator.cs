using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public enum OperatorType
    {
        Plus = 200, Minus = 201, Prod = 100, Div = 101, Equals = 300
    }

    public delegate Value OperatorApplication(Value Arg1, Value Arg2);

    public class Operator : ICallable
    {
        public int Priority { get { return (int)type; } }
        public byte Arity;
        public ValueKind operand1;
        public ValueKind operand2;
        public OperatorType type;
        //public OperatorApplication application;

        public Operator(byte Arity, ValueKind operand1, ValueKind operand2, OperatorType type)
        {
            this.Arity = Arity;
            this.operand1 = operand1;
            this.operand2 = operand2;
            this.type = type;
        }

        public void Call(IEnumerable<Value> Args, out Value Result)
        {
            var en = Args.GetEnumerator(); en.MoveNext();
            var first = en.Current;
            var second = en.MoveNext() ? en.Current : null;
            throw new NotImplementedException();
            //Result = application(first, second);
        }
    }

    public static class BuildinIntegralOperators
    {
        /*
        public static Operator plus = new Operator(2, ValueKind.Integral, ValueKind.Integral, (x, y) => { return new IntegralValue((x as IntegralValue).value + (y as IntegralValue).value); } );
        public static Operator times = new Operator(2, ValueKind.Integral, ValueKind.Integral, (x, y) => { return new IntegralValue((x as IntegralValue).value * (y as IntegralValue).value); });
        public static Operator minus = new Operator(2, ValueKind.Integral, ValueKind.Integral, (x, y) => { return new IntegralValue((x as IntegralValue).value - (y as IntegralValue).value); });
        */
    }
}
