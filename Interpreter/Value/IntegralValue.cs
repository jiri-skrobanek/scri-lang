using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Value
{
    public struct IntegralValue : IValue
    {
        public int Value;

        public IntegralValue(int value)
        {
            this.Value = value;
        }

        public ValueKind ValueKind { get { return ValueKind.Integral; } }

        public bool GetTruthValue()
        {
            return this;
        }

        public override string ToString()
        {
            return Value.ToString();
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
            return IV.Value;
        }

        public static implicit operator bool(IntegralValue IV)
        {
            return IV.Value != 0;
        }

        public static IntegralValue operator +(IntegralValue l, IntegralValue r)
        {
            return l.Value + r.Value;
        }

        public static IntegralValue operator -(IntegralValue l, IntegralValue r)
        {
            return l.Value - r.Value;
        }

        public static IntegralValue operator *(IntegralValue l, IntegralValue r)
        {
            return l.Value * r.Value;
        }

        public static IntegralValue operator /(IntegralValue l, IntegralValue r)
        {
            return l.Value / r.Value;
        }

        public static IntegralValue operator >(IntegralValue l, IntegralValue r)
        {
            return l.Value > r.Value;
        }

        public static IntegralValue operator <(IntegralValue l, IntegralValue r)
        {
            return l.Value < r.Value;
        }

        public static IntegralValue operator ==(IntegralValue l, IntegralValue r)
        {
            return l.Value == r.Value;
        }

        public static IntegralValue operator !=(IntegralValue l, IntegralValue r)
        {
            return !(l == r);
        }

        #endregion
    }
}
