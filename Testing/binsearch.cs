using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
    partial class Program {

        static void TestBinsearch()
        {
            var source = @"
binfind @ (data, count, item) 
(
	if count ? 0 then (return none;);
	left = 0;
	right = count - 1;
	while left < right do
	(
		middle = (left + right) / 2 + 1;
		if data(middle) ? item then
		(
			return middle;
		);
		if data(middle) > item then
		(
			right = middle - 1;
		)
		else 
		(
			left = middle + 1;
		);
	);
	if data(left) ? item then (return left;) else (return none;);
);

nums = vector();
i = 0;
while i ! 10 do 
(
    nums(i, 10 * i);
    i = i + 1;
);

k = binfind(nums, 10, 70);
";

            var env = new Interpreter.Environment.DefaultEnvironment();
            var block = Parser.Parser.ParseCode(source);
            env.Execute(block);
            Assert(((Interpreter.IntegralValue)env.GlobalScope["k"]).value == 7);
        }
    }
}
