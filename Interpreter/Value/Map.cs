using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Value
{

    public class Map : IValue, ICallable
    {
        private readonly Dictionary<IValue, IValue> items = new Dictionary<IValue, IValue>();

        public Invocation Call => _call;

        void _call(IList<IValue> Args, out IValue result)
        {
            var len = Args.Count;
            if (len == 1)
            {
                if (Args[0] is IntegralValue)
                {
                    if (items.ContainsKey(Args[1]))
                    {
                        result = items[(IntegralValue)Args[0]];
                    }
                    else
                    {
                        result = new None();
                    }
                }
                else
                {
                    throw new Exception("Invalid map manipulation exception.");
                }
            }
            else if (len == 2)
            {
                result = new None();
                if (Args[1] is None)
                {
                    items.Remove(Args[0]);
                }
                else
                {
                    items[Args[0]] = Args[1];
                }
            }
            else
            {
                throw new Exception("Invalid map manipulation exception.");
            }
        }

        public bool GetTruthValue()
        {
            return items.Count > 0;
        }

        public override string ToString()
        {
            return "Map";
        }

        public ValueKind ValueKind { get { return ValueKind.Vector; } }
    }
}
