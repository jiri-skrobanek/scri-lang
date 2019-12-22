using Interpreter.Value;
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
            Assert(((IntegralValue)env.GlobalScope["a"]).Value == 6);
            Assert(((IntegralValue)env.GlobalScope["b"]).Value == -2);
            Assert(((IntegralValue)env.GlobalScope["c"]).Value == 7);
        }
    }
}
