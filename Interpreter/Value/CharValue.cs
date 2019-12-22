using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Value
{

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

        #region Operators

        public static implicit operator CharValue(char c)
        {
            return new CharValue(c);
        }

        public static implicit operator char(CharValue cv)
        {
            return cv.value;
        }

        public static IntegralValue operator ==(CharValue l, CharValue r)
        {
            return l.value == r.value;
        }

        public static IntegralValue operator !=(CharValue l, CharValue r)
        {
            return !(l == r);
        }

        #endregion Operators
    }
}
