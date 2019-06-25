using System;
using System.Collections.Generic;
using Interpreter;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TestAddition();
        }

        static void TestAddition()
        {
            var v = new Interpreter.OperatorEvaluation()
            {
                left_arg = new ConstantExpression() { value = new IntegralValue(7) },
                right_arg = new ConstantExpression() { value = new IntegralValue(13) }, 
                @operator = OperatorType.Plus
            };
            var l = v.Evaluate(new Scope());
            var v2 = new Interpreter.OperatorEvaluation()
            {
                left_arg = v,
                right_arg = new VariableExpression { variableName = "zuby" },
                @operator = OperatorType.Prod
            };
            var l2 = v2.Evaluate(new Scope { Global = true, Names = new Dictionary<string, IValue> { ["zuby"] = new IntegralValue(11) } });
            int i = 0;
        }
    }
}
