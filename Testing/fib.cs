using System;
using System.Collections.Generic;
using System.Text;

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

            var env = new Interpreter.Environment();
            var block = Parser.Parser.ParseCode(source);
            env.Execute(block);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["f0"]).value == 0);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["f1"]).value == 1);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["f2"]).value == 1);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["f3"]).value == 2);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["f4"]).value == 3);
        }
    }
}
