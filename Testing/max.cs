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
";
            var env = new Interpreter.Environment();
            var block = Parser.Parser.ParseCode(source);
            env.Execute(block);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["a"]).value == 6);
        }
    }
}
