using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
    partial class Program {

        static void TestNim()
        {
            var source = @"
nim @ (matches) 
(
	whole = (matches / 4) * 4;
	remove = matches - whole;
	if remove ! 0 then (return remove;) else (return 1;);
);
m0 = 21;
m1 = m0 - nim(m0);
m2 = m1 - nim(m1);
m3 = m2 - nim(m2);
m4 = m3 - nim(m3);
m5 = m4 - nim(m4);
";
            var env = new Interpreter.Environment.DefaultEnvironment();
            var block = Parser.Parser.ParseCode(source);
            env.Execute(block);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["m1"]).value == 20);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["m3"]).value == 16);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["m5"]).value == 12);
        }
    }
}
