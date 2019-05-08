using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public abstract class Value
    {
        public ValueKind valueKind;

        public virtual bool GetTruthValue()
        {
            throw new NotImplementedException();
        }
    }

    public class IntegralValue : Value
    {
        public int value;
        
        public IntegralValue(int value)
        {
            this.value = value;
            valueKind = ValueKind.Integral;
        }

        public override bool GetTruthValue()
        {
            return value != 0;
        }
    }

    public class CharValue : Value
    {
        public char value;

        public CharValue(char value)
        {
            this.value = value;
            valueKind = ValueKind.Char;
        }

        public override bool GetTruthValue()
        {
            return value != 0;
        }
    }
}
