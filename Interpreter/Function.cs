using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    class Function : INamable
    {
        public String Name { get; set; }
        public List<String> Arguments;
        public Scope creation_scope;
        public Block body;

        public void Call(IEnumerable<Value> Args, out Value result)
        {
            var en = Args.GetEnumerator();

            var s = new Scope { ParentScope = creation_scope };

            for (int i = 0; i < Arguments.Count; i++, en.MoveNext())
            {
                var v = new Variable() { value = en.Current };
                s.Names.Add(Arguments[i], v);
            }

            var execr = body.Execute(s);
            if (execr.GetType() == typeof(ReturnResult))
            {
                result = (execr as ReturnResult).result;
            }
            else
            {
                result = null;
            }
        }
    }
}
