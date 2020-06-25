using System;
using System.Collections.Generic;
using System.IO;
using Interpreter;

namespace Testing
{
    partial class Program
    {
        static void Main(string[] args)
        {
            TestMax();
            TestNim();
            TestFib();
            TestBinsearch();
            TestMoving();
        }

        static void Assert(bool b)
        {
            if (!b) throw new Exception("Incorrect result!");
        }
        
    }
}
