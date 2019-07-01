using System;
using System.Collections.Generic;
using System.Text;
using OpDict = System.Collections.Generic.Dictionary<(Interpreter.OperatorType, Interpreter.ValueKind?, Interpreter.ValueKind?), Interpreter.OperatorApplication>;

namespace Interpreter
{
    public enum OperatorType
    {
        Plus = 200, Minus = 201, Prod = 100, Div = 101, Equals = 300, NEQ = 301, Greater = 302, Lesser = 303, And = 401, Or = 400, FDef = 500, Assign = 501
    }

    public delegate IValue OperatorApplication(IValue Arg1, IValue Arg2);

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

        public Invocation Call { get { return _call; } }

        void _call(IEnumerable<IValue> Args, out IValue Result)
        {
            var en = Args.GetEnumerator(); en.MoveNext();
            var first = en.Current;
            var second = en.MoveNext() ? en.Current : null;
            Result = GetApplication(type, operand1, operand2)(first, second);
        }

        private static readonly OpDict SpecificOperators = new OpDict
        {

            // Intergral Operators:

            [(OperatorType.Plus, ValueKind.Integral ,ValueKind.Integral)] = (x,y) => { return (IntegralValue)x + (IntegralValue)y; },
            [(OperatorType.Minus, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { return (IntegralValue)x - (IntegralValue)y; },
            [(OperatorType.Prod, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { return (IntegralValue)x * (IntegralValue)y; },
            [(OperatorType.Div, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { if ((IntegralValue)y == 0) { return new None(); } return (IntegralValue)x / (IntegralValue)y; },
            [(OperatorType.Greater, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { return ((IntegralValue)x > (IntegralValue)y); },
            [(OperatorType.Lesser, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { return (IntegralValue)x < (IntegralValue)y; },
            [(OperatorType.Equals, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { return ((IntegralValue)x == (IntegralValue)y); },
            [(OperatorType.NEQ, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { return (IntegralValue)x != (IntegralValue)y; },
            [(OperatorType.And, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { return new IntegralValue(0 == (IntegralValue)x || 0 == (IntegralValue)y ? 0 : 1); },
            [(OperatorType.Or, ValueKind.Integral, ValueKind.Integral)] = (x, y) => { return new IntegralValue(0 == (IntegralValue)x && 0 == (IntegralValue)y ? 0 : 1); },

            // Unary Integral Operators:

            [(OperatorType.Plus, null, ValueKind.Integral)] = (x, y) => { return y; },
            [(OperatorType.Minus, null, ValueKind.Integral)] = (x, y) => { return (IntegralValue)(-((IntegralValue)y).value); },

            // Equality operators for other:

            [(OperatorType.Equals, ValueKind.Char, ValueKind.Char)] = (x, y) => { return (CharValue)x == (CharValue)y; },
            [(OperatorType.Equals, ValueKind.None, ValueKind.None)] = (x, y) => { return new IntegralValue(1); },
            [(OperatorType.Equals, ValueKind.Function, ValueKind.Function)] = (x, y) => { return (IntegralValue)((Function)x == (Function)y ? 1 : 0); },
            [(OperatorType.Equals, ValueKind.Vector, ValueKind.Vector)] = (x, y) => { return (IntegralValue)((Vector)x == (Vector)y ? 1 : 0); },
            [(OperatorType.Equals, ValueKind.Map, ValueKind.Map)] = (x, y) => { return (IntegralValue)((Map)x == (Map)y ? 1 : 0); },

            // Inequality operators for other:

            [(OperatorType.NEQ, ValueKind.Char, ValueKind.Char)] = (x, y) => { return (CharValue)x != (CharValue)y; },
            [(OperatorType.Equals, ValueKind.None, ValueKind.None)] = (x, y) => { return new IntegralValue(0); },
            [(OperatorType.Equals, ValueKind.Function, ValueKind.Function)] = (x, y) => { return (IntegralValue)((Function)x == (Function)y ? 0 : 1); },
            [(OperatorType.Equals, ValueKind.Vector, ValueKind.Vector)] = (x, y) => { return (IntegralValue)((Vector)x == (Vector)y ? 0 : 1); },
            [(OperatorType.Equals, ValueKind.Map, ValueKind.Map)] = (x, y) => { return (IntegralValue)((Map)x == (Map)y ? 0 : 1); },
        };

        public static OperatorApplication GetApplication(OperatorType type, ValueKind? left, ValueKind? right)
        {
            if(SpecificOperators.ContainsKey((type, left, right)))
            {
                return SpecificOperators[(type, left, right)];
            }
            if(type == OperatorType.And)
            {
                return (x, y) => { return new IntegralValue( x.GetTruthValue() && y.GetTruthValue() ? 1 : 0); };
            }
            if(type == OperatorType.Or)
            {
                return (x, y) => { return new IntegralValue(x.GetTruthValue() || y.GetTruthValue() ? 1 : 0); };
            }
            return (x, y) => { return new None(); };
        }
    }
}
