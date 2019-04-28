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
            var v = new Interpreter.OperatorEvaluation() { args = new List<IExpression>() { new ConstantExpression() { value = new IntegralValue(7) }, new ConstantExpression() { value = new IntegralValue(13) } }, @operator = BuildinIntegralOperators.plus };
            v.Evaluate(new Scope());
            int i = 0;
        }
    }
}
