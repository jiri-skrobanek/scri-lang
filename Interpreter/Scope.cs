using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public class Scope
    {
        public bool Global = false;
        public Dictionary<String, NameCollection> Names = new Dictionary<string, NameCollection>();
        public Scope ParentScope;

        public Function FindFunction(string Name, byte ArgCount)
        {
            if(Names.ContainsKey(Name))
            {
                return Names[Name].functions[ArgCount];
            }
            else
            {
                return ParentScope.FindFunction(Name,ArgCount);
            }
        }

        public Variable FindVariable(string Name)
        {
            if(Names.ContainsKey(Name))
            {
                return Names[Name].variable;
            }
            else
            {
                return ParentScope.FindVariable(Name);
            }
        }

        public void AssignValue(string Name, Value v)
        {
            if (Names.ContainsKey(Name))
            {
                Names[Name].variable.value = v;
            }
            else
            {
                if(!ParentScope.assign(Name, v))
                {
                    Names.Add(Name, new NameCollection() { variable = new Variable() { Name = Name, value = v } });
                }
            }
        }

        private bool assign(string Name, Value v)
        {
            if (Names.ContainsKey(Name))
            {
                Names[Name].variable.value = v;
                return true;
            }
            else if(Global)
            { return false; }
            else
            {
                return ParentScope.assign(Name, v);
            }
        }
    }
}
