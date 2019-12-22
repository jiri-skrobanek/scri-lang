namespace Interpreter.Value
{
    /// <summary>
    /// Overlying type for values
    /// </summary>
    public interface IValue
    {
        ValueKind ValueKind { get; }

        bool GetTruthValue();
    }

    public enum ValueKind
    {
        None, Char, Integral, Vector, Map, Function, Builtin
    }
}
