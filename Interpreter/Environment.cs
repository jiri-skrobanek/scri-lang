using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    /// <summary>
    /// Provider of built-in functions. (Base class)
    /// </summary>
    public class Environment
    {
        public Environment()
        {
            GlobalScope = new Scope(this)
            {
                Names = new Dictionary<string, IValue>()
                {
                    ["vector"] = new Buildin() { Call = getNewVector },
                    ["map"] = new Buildin() { Call = getNewMap },
                    ["char"] = new Buildin() { Call = makeChar }
                }
            };
        }

        public delegate void PrintDirective(string Text);

        public PrintDirective PrintText { get; set; } = x => { };

        public Scope GlobalScope { get; set; }

        public void Execute(Block block)
        {
            block.Execute(this);
        }

        #region Build-in functions

        protected static void getNewVector(IList<IValue> Args, out IValue result)
        {
            while(Args.Count > 0 && Args[Args.Count - 1] is None)
            {
                Args.RemoveAt(Args.Count - 1);
            }
            result = new Vector(Args);
        }

        protected static void getNewMap(IList<IValue> Args, out IValue result)
        {
            result = new Map();
        }

        protected static void makeChar(IList<IValue> Args, out IValue result)
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

        protected static void makeInt(IList<IValue> Args, out IValue result)
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
