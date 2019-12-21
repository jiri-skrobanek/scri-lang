namespace Interpreter.Environment
{
    public interface IEnvironment
    {
        Scope GlobalScope { get; set; }
        DefaultEnvironment.PrintDirective PrintText { get; set; }

        void Execute(Block block);
    }
}