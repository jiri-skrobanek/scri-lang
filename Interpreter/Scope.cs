using System;
using System.Collections.Generic;
using System.Text;
using NameCollection = System.Collections.Generic.Dictionary<string, Interpreter.IValue>;

namespace Interpreter
{
    public class Scope
    {
        public bool Global { get { return ParentScope != null; } }
        public NameCollection Names = new NameCollection();
        public Scope ParentScope { get; } = null;
        public Environment Environment { get; }

        public Scope(Environment environment)
        {
            Environment = environment;
        }

        public Scope(Scope parent_scope)
        {
            ParentScope = parent_scope;
            Environment = parent_scope.Environment;
        }

        public IValue this[string Name]
        {
            get
            {
                if (Names.ContainsKey(Name))
                {
                    return Names[Name];
                }
                else if (!Global)
                {
                    return ParentScope[Name];
                }
                else
                {
                    return new None();
                }
            }
            set
            {
                if (Names.ContainsKey(Name))
                {
                    if (value is None)
                    {
                        Names.Remove(Name);
                    }
                    else
                    {
                        Names[Name] = value;
                    }
                }
                else
                {
                    if (!ParentScope.assign_only(Name, value))
                    {
                        Names.Add(Name, value);
                    }
                }
            }
        }

        private bool assign_only(string Name, IValue v)
        {
            if (Names.ContainsKey(Name))
            {
                if (v is None)
                {
                    Names.Remove(Name);
                }
                else
                { Names[Name] = v;
                }
                return true;
            }
            else if(Global)
            { return false; }
            else
            {
                return ParentScope.assign_only(Name, v);
            }
        }
    }
}
