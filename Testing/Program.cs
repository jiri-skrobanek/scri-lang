using System;
using System.Collections.Generic;
using Interpreter;

namespace Testing
{
    partial class Program
    {
        static void Main(string[] args)
        {
            TestMax();
            //Console.WriteLine("Hello World!");
            //TestAddition();
        }

        static void Assert(bool b)
        {
            if (!b) throw new Exception("Incorrect result!");
        }

        static void TestAddition()
        {
            var v = new Interpreter.OperatorEvaluation()
            {
                left_arg = new ConstantExpression() { value = new IntegralValue(7) },
                right_arg = new ConstantExpression() { value = new IntegralValue(13) }, 
                @operator = OperatorType.Plus
            };
            var l = v.Evaluate(new Scope(new Interpreter.Environment()));
            var v2 = new Interpreter.OperatorEvaluation()
            {
                left_arg = v,
                right_arg = new VariableExpression { variableName = "zuby" },
                @operator = OperatorType.Prod
            };
            var l2 = v2.Evaluate(new Scope(new Interpreter.Environment()) { Names = new Dictionary<string, IValue> { ["zuby"] = new IntegralValue(11) } });
            int i = 0;
        }
        
    }
}
