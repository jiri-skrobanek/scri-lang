using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public enum OperatorToken
    {
        Plus = 200, Minus = 201, Prod = 100, Div = 101, Equals = 300
    }

    public static class OperatorPriorities
    {
        public static bool HasGreaterPriority(OperatorToken o1, OperatorToken o2)
        {
            return (int)o1 >> 2 < (int)o2 >> 2;
        }

        public static bool HasGreaterPriority(int priority, OperatorToken o2)
        {
            return priority >> 2 > (int)o2 >> 2;
        }
    }
}
