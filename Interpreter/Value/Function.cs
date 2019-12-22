using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Value
{
    public class Function : IValue, ICallable
    {
        public IList<String> Arguments;
        public Scope creation_scope;
        public Block body;

        public Function(IList<String> Args, Block Body, Scope creation_scope)
        {
            this.Arguments = Args;
            this.body = Body;
            this.creation_scope = creation_scope;
        }

        public Invocation Call { get { return _call; } }

        void _call(IList<IValue> Args, out IValue result)
        {
            var en = Args.GetEnumerator();

            var s = new Scope(creation_scope);

            for (int i = 0; i < Arguments.Count; i++)
            {
                en.MoveNext();
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
}
