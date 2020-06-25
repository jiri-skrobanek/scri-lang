using Interpreter.Value;
using Interpreter.Environment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
    partial class Program
    {

        private class ValueIteratingEnvironment : DefaultEnvironment
        {
            [BuiltinVariable("goal")]
            public IntegralValue goal = 2047;

            public int current = 1;

            [Interpreter.Environment.BuiltinFunction("move_next")]
            public void MoveNext(IList<IValue> args, out IValue result)
            {
                current *= 4;
                current += 3;

                result = (IntegralValue) current;
            }

            public int result = 0;

            [Interpreter.Environment.BuiltinFunction("set_result")]
            public void SetResult(IList<IValue> args, out IValue result)
            {
                if (args.Count != 1) throw new ArgumentException();
                if (args[0] is IntegralValue i) this.result = i;
                else throw new ArgumentException();

                result = new None();
            }
        }

        static void TestMoving()
        {
            var source = @"
i = 0;
while move_next() ! goal do 
(
    i = i + 1;
);
set_result(i);
";
            var env = new ValueIteratingEnvironment();
            var block = Parser.Parser.ParseCode(source);
            env.Execute(block);
            Assert(env.result == 4);
        }
    }
}
