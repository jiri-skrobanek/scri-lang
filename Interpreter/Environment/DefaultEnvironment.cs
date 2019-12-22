using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using Interpreter.Value;

namespace Interpreter.Environment
{
    /// <summary>
    /// Provider of built-in functions. (Default implementation)
    /// </summary>
    public class DefaultEnvironment : IEnvironment
    {
        public DefaultEnvironment()
        {
            var dictionary = new Dictionary<string, IValue>();

            foreach (var method in ExtractMethods()) dictionary.Add(method.Name, method.Value);
            foreach (var variable in ExtractVariables()) dictionary.Add(variable.Name, variable.Value);

            GlobalScope = new Scope(this)
            {
                Names = dictionary
            };
        }

        private IEnumerable<(string Name, IValue Value)> ExtractMethods()
        {
            var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var builtins = methods.Where(x => isBuiltinMethod(x));

            var methodList = new List<(string Name, IValue Value)>();

            foreach (var b in builtins)
            {
                var info = b.GetCustomAttribute<BuiltinFunction>();
                if (info != null)
                {
                    Invocation inv = (Invocation)Delegate.CreateDelegate(typeof(Invocation), b);

                    methodList.Add((info.Name, new Value.BuiltinFunction(inv)));
                }
            }

            return methodList;

            static bool isBuiltinMethod(MethodInfo method)
            {
                var pars = method.GetParameters();
                if (pars.Length != 2) return false;
                if (pars[0].ParameterType != typeof(IList<IValue>)) return false;
                if (pars[1].ParameterType.FullName != typeof(Interpreter.Value.IValue).FullName + "&") return false;
                
                return pars[1].IsOut;
            }
        }

        private IEnumerable<(string Name, IValue Value)> ExtractVariables()
        {
            var variables = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            var varList = new List<(string Name, IValue Value)>();

            foreach (var b in variables)
            {
                if (typeof(IValue).IsAssignableFrom(b.FieldType))
                {
                    var info = b.GetCustomAttribute<BuiltinVariable>();
                    if (info != null)
                    {
                        varList.Add((info.Name, (IValue)b.GetValue(this)));
                    }
                }
            }

            return varList;
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
                if (iv.Value > 255 || iv.Value < 0) { result = new None(); }
                else
                {
                    result = new CharValue((char)iv.Value);
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

        [BuiltinVariable("version")]
        private static IntegralValue version = new IntegralValue(1); 
    }
}
