using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace Interpreter.Environment
{
    /// <summary>
    /// Provider of built-in functions. (Default implementation)
    /// </summary>
    public class DefaultEnvironment : IEnvironment
    {
        public DefaultEnvironment()
        {
            GlobalScope = new Scope(this)
            {
                Names = ExtractMethods()
            };
        }

        private Dictionary<string, IValue> ExtractMethods()
        {
            var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var builtins = methods.Where(x => isBuiltinMethod(x)).ToArray();

            var methodDictionary = new Dictionary<string, IValue>();

            foreach (var b in builtins)
            {
                var info = b.GetCustomAttribute<BuiltinFunction>();
                if (info != null)
                {
                    Invocation inv = (Invocation)Delegate.CreateDelegate(typeof(Invocation), b);

                    methodDictionary.Add(info.Name, new Interpreter.BuiltinFunction(inv));
                }
            }

            return methodDictionary;

            static bool isBuiltinMethod(MethodInfo method)
            {
                var pars = method.GetParameters();
                if (pars.Length != 2) return false;
                if (pars[0].ParameterType != typeof(IList<IValue>)) return false;
                if (pars[1].ParameterType.FullName != "Interpreter.IValue&") return false;
                
                return pars[1].IsOut;
            }
        }

        public delegate void PrintDirective(string Text);

        public PrintDirective PrintText { get; set; } = x => { };

        public Scope GlobalScope { get; set; }

        public void Execute(Block block)
        {
            block.Execute(this);
        }

        #region Build-in functions

        [BuiltinFunction("vector")]
        protected static void getNewVector(IList<IValue> Args, out IValue result)
        {
            while (Args.Any() && Args[Args.Count - 1] is None)
            {
                Args.RemoveAt(Args.Count - 1);
            }
            result = new Vector(Args);
        }

        [BuiltinFunction("map")]
        protected static void getNewMap(IList<IValue> Args, out IValue result)
        {
            result = new Map();
        }

        [BuiltinFunction("char")]
        protected static void makeChar(IList<IValue> Args, out IValue result)
        {
            if (Args.Count >= 1 && Args[1] is IntegralValue iv)
            {
                if (iv.value > 255 || iv.value < 0) { result = new None(); }
                else
                {
                    result = new CharValue((char)iv.value);
                }
            }
            else
            {
                result = new None();
            }

        }

        [BuiltinFunction("int")]
        protected static void makeInt(IList<IValue> Args, out IValue result)
        {
            if (Args.Any() && Args[1] is CharValue cv)
            {
                result = new IntegralValue(cv.value);
            }
            else if (Args.Any() && Args[1] is IntegralValue iv)
            {
                result = iv;
            }
            else
            {
                result = new None();
            }

        }

        #endregion Build-in functions
    }
}
