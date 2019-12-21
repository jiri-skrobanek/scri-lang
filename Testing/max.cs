using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
    partial class Program {

        static void TestMax()
        {
            var source = @"
max @ (first, second)
(
	if first > second then
    (
		return first;
	) 
	else 
	(
		return second;
	);
);
a = max(5, 6);
b = max(-2, -3);
c = max(a, b + 9);
";
            var env = new Interpreter.Environment.DefaultEnvironment();
            var block = Parser.Parser.ParseCode(source);
            env.Execute(block);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["a"]).value == 6);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["b"]).value == -2);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["c"]).value == 7);
        }
    }
}
