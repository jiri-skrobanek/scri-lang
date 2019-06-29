using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{


    public static class Environment
    {
        public delegate void PrintDirective(String Text);

        public static PrintDirective PrintText { get; set; } = x => { };

        public static Scope GlobalScope { get; set; } = new Scope()
        {
            Global = true,
            Names = new Dictionary<string, IValue>()
            {
                ["vector"] = new Buildin() { Call = getNewVector },
                ["map"] = new Buildin() { Call = getNewMap },
                ["char"] = new Buildin() { Call = makeChar }
            }
        };

        #region Build-in functions

        private static void getNewVector(IList<IValue> Args, out IValue result)
        {
            while(Args.Count > 0 && Args[Args.Count - 1] is None)
            {
                Args.RemoveAt(Args.Count - 1);
            }
            result = new Vector(Args);
        }

        private static void getNewMap(IList<IValue> Args, out IValue result)
        {
            result = new Map();
        }

        private static void makeChar(IList<IValue> Args, out IValue result)
        {
            if (Args.Count >= 1 && Args[1] is IntegralValue iv)
            {
                if(iv.value > 255 || iv.value < 0) { result = new None(); }
                else
                {
                    result = new CharValue((char)iv.value);
                }
            }
            else
            {
                result = new None();
            }

        }

        private static void makeInt(IList<IValue> Args, out IValue result)
        {
            if (Args.Count >= 1 && Args[1] is CharValue cv)
            {
                result = new IntegralValue(cv.value);
            }
            else
            {
                result = new None();
            }

        }

        #endregion Build-in functions
    }
}
