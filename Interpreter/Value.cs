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
            return l + r;
        }

        public static IntegralValue operator -(IntegralValue l, IntegralValue r)
        {
            return l - r;
        }

        public static IntegralValue operator *(IntegralValue l, IntegralValue r)
        {
            return l * r;
        }

        public static IntegralValue operator /(IntegralValue l, IntegralValue r)
        {
            return l / r;
        }

        public static bool operator >(IntegralValue l, IntegralValue r)
        {
            return l > r;
        }

        public static bool operator <(IntegralValue l, IntegralValue r)
        {
            return l < r;
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

        public void Call(IEnumerable<IValue> Args, out IValue result)
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

        public ValueKind ValueKind { get { return ValueKind.Function; } }
    }
}
