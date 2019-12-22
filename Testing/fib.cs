using System;
using System.Collections.Generic;
using System.Text;
using Interpreter.Value;

namespace Testing
{
    partial class Program {

        static void TestFib()
        {
            var source = @"
fib @ (amount) 
(
	if amount ? 0 then (return 0;);
	if amount ? 1 then (return 1;);
	return fib(amount - 1) + fib(amount - 2);
);
f0 = fib(0);
f1 = fib(1);
f2 = fib(2);
f3 = fib(3);
f4 = fib(4);
";

            var env = new Interpreter.Environment.DefaultEnvironment();
            var block = Parser.Parser.ParseCode(source);
            env.Execute(block);
            Assert(((IntegralValue)env.GlobalScope["f0"]).Value == 0);
            Assert(((IntegralValue)env.GlobalScope["f1"]).Value == 1);
            Assert(((IntegralValue)env.GlobalScope["f2"]).Value == 1);
            Assert(((IntegralValue)env.GlobalScope["f3"]).Value == 2);
            Assert(((IntegralValue)env.GlobalScope["f4"]).Value == 3);
        }
    }
}
