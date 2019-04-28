using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public class Function : INamable, ICallable
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
                s.AssignValue(Arguments[i], en.Current);
            }

            var execr = body.Execute(s);
            if (execr.resultType == ResultType.Return)
            {
                result = (execr as ReturnResult).result;
            }
            else if(execr.resultType == ResultType.Performed)
            {
                result = null;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
