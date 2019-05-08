using System;
using System.Collections.Generic;
using System.Text;
using Interpreter;

namespace Parser
{

    public static class OperatorPriorities
    {
        public static bool HasGreaterPriority(OperatorType o1, OperatorType o2)
        {
            return (int)o1 >> 2 < (int)o2 >> 2;
        }

        public static bool HasGreaterPriority(int priority, OperatorType o2)
        {
            return priority >> 2 > (int)o2 >> 2;
        }
    }
}
