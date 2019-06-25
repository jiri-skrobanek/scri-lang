using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public interface IValue
    {
        ValueKind ValueKind { get; }

        bool GetTruthValue();
    }

    public struct None : IValue
    {
        public ValueKind ValueKind { get { return ValueKind.None; } }

        public bool GetTruthValue()
        {
            return false;
        }

        public override string ToString()
        {
            return "None";
        }
    }

    public struct IntegralValue : IValue
    {
        public int value;
        
        public IntegralValue(int value)
        {
            this.value = value;
        }

        public ValueKind ValueKind { get { return ValueKind.Integral; } }

        public bool GetTruthValue()
        {
            return this;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        #region Conversion Operators

        public static implicit operator IntegralValue(int num)
        {
            return new IntegralValue(num);
        }

        public static implicit operator IntegralValue(bool b)
        {
            return new IntegralValue(b ? 1 : 0);
        }

        public static implicit operator int(IntegralValue IV)
        {
            return IV.value;
        }

        public static implicit operator bool(IntegralValue IV)
        {
            return IV.value != 0;
        }

        public static IntegralValue operator +(IntegralValue l, IntegralValue r)
        {
            return l.value + r.value;
        }

        public static IntegralValue operator -(IntegralValue l, IntegralValue r)
        {
            return l.value - r.value;
        }

        public static IntegralValue operator *(IntegralValue l, IntegralValue r)
        {
            return l.value * r.value;
        }

        public static IntegralValue operator /(IntegralValue l, IntegralValue r)
        {
            return l.value / r.value;
        }

        public static IntegralValue operator >(IntegralValue l, IntegralValue r)
        {
            return l.value > r.value;
        }

        public static IntegralValue operator <(IntegralValue l, IntegralValue r)
        {
            return l.value < r.value;
        }

        public static IntegralValue operator ==(IntegralValue l, IntegralValue r)
        {
            return l.value == r.value;
        }

        public static IntegralValue operator !=(IntegralValue l, IntegralValue r)
        {
            return !(l == r);
        }

        #endregion
    }

    public struct CharValue : IValue
    {
        public char value;

        public CharValue(char value)
        {
            this.value = value;
        }

        public bool GetTruthValue()
        {
            return value != 0;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public ValueKind ValueKind { get { return ValueKind.Char; } }
    }

    public class Function : IValue, ICallable
    {
        public List<String> Arguments;
        public Scope creation_scope;
        public Block body;

        public Function(List<String> Args, Block Body, Scope creation_scope)
        {
            this.Arguments = Args;
            this.body = Body;
            this.creation_scope = creation_scope;
        }

        public Invocation Call { get { return _call; } }

        void _call(IList<IValue> Args, out IValue result)
        {
            var en = Args.GetEnumerator();

            var s = new Scope { ParentScope = creation_scope };

            for (int i = 0; i < Arguments.Count; i++, en.MoveNext())
            {
                s[Arguments[i]] = en.Current;
            }

            var execr = body.Execute(s);
            if (execr.resultType == ResultType.Return)
            {
                result = (execr as ReturnResult).result;
            }
            else if (execr.resultType == ResultType.Performed)
            {
                result = null;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool GetTruthValue()
        {
            return true;
        }

        public override string ToString()
        {
            return "Function";
        }

        public ValueKind ValueKind { get { return ValueKind.Function; } }
    }

    public class Vector : IValue, ICallable
    {
        private List<IValue> items = new List<IValue>();

        public Vector(IList<IValue> Args)
        {
            items.AddRange(Args);
        }

        public Invocation Call { get { return _call; } }

        void _call(IList<IValue> Args, out IValue result)
        {
            var len = Args.Count;
            if(len == 1)
            {
                if (Args[0] is IntegralValue)
                {
                    int Index = (IntegralValue)Args[0];
                    if(Index < items.Count)
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
            else if (len==2)
            {
                if (Args[0] is IntegralValue)
                {
                    result = new None();
                    int Index = (IntegralValue)Args[0];
                    if (Index < 0)
                    {
                        if(Index != -1)
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
                        else if(!(Args[1] is None))
                        {
                            while(items.Count < Index)
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

    public class Map : IValue, ICallable
    {
        private Dictionary<IValue,IValue> items = new Dictionary<IValue, IValue>();

        public Invocation Call { get { return _call; } }

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
            return "Vector";
        }

        public ValueKind ValueKind { get { return ValueKind.Vector; } }
    }

    public class Buildin : IValue, ICallable
    {
        public ValueKind ValueKind { get { return ValueKind.Buildin; } }

        public Invocation Call { get; set; }

        public bool GetTruthValue()
        {
            return true;
        }
    }
}
