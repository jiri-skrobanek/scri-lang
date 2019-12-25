using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Value
{

    public class Vector : IValue, ICallable
    {
        private readonly List<IValue> items = new List<IValue>();

        public Vector(IList<IValue> Args)
        {
            items.AddRange(Args);
        }

        public int Length => items.Count;

        public Invocation Call => _call;

        void _call(IList<IValue> Args, out IValue result)
        {
            var len = Args.Count;
            if (len == 1)
            {
                if (Args[0] is IntegralValue)
                {
                    int Index = (IntegralValue)Args[0];
                    if (Index < items.Count)
                    {
                        result = items[Index];
                    }
                    else
                    {
                        result = new None();
                    }
                }
                else
                {
                    throw new Exception("Invalid list manipulation exception.");
                }
            }
            else if (len == 2)
            {
                if (Args[0] is IntegralValue)
                {
                    result = new None();
                    int Index = (IntegralValue)Args[0];
                    if (Index < 0)
                    {
                        if (Index != -1)
                        {
                            throw new Exception("Invalid list manipulation exception.");
                        }
                        items.Add(Args[1]);
                    }
                    else
                    {
                        if (items.Count > Index)
                        {
                            items[Index] = Args[1];

                            // Remove Nones from the end:
                            while (items.Count > 0 && items[items.Count - 1] is None)
                            {
                                items.RemoveAt(items.Count - 1);
                            }
                        }
                        else if (!(Args[1] is None))
                        {
                            while (items.Count < Index)
                            {
                                items.Add(new None());
                            }
                            items.Add(Args[1]);
                        }
                    }
                }
                else
                {
                    throw new Exception("Invalid list manipulation exception.");
                }
            }
            else
            {
                throw new Exception("Invalid list manipulation exception.");
            }
        }

        public bool GetTruthValue()
        {
            return items.Count > 0;
        }

        public override string ToString()
        {
            return "Vector";
        }

        public ValueKind ValueKind { get { return ValueKind.Vector; } }
    }
}
