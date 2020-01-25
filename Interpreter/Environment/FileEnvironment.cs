using Interpreter.Value;
using System.Collections.Generic;
using System.Text;

namespace Interpreter.Environment
{
    /// <summary>
    /// An environment for interfacing with a file.
    /// </summary>
    public class FileEnvironment : DefaultEnvironment
    {
        private readonly System.IO.FileStream stream;

        public FileEnvironment(System.IO.FileStream fileStream)
        {
            stream = fileStream;
            if (fileStream.CanRead)
            {
                GlobalScope["read"] = new Interpreter.Value.BuiltinFunction(getChar);
            }
            if (fileStream.CanWrite)
            {
                // Prints the string to the file.
                PrintText = (x) => { foreach (byte b in Encoding.ASCII.GetBytes(x)) { stream.WriteByte(b); } };
            }
        }

        /// <summary>
        /// Returns next character from the file or none when at the end of file.
        /// </summary>
        private void getChar(IList<IValue> Args, out IValue result)
        {
            var b = stream.ReadByte();
            result = (b == -1) ? (IValue)new None() : new CharValue((char)b);
        }

        /// <summary>
        /// Sets the position in the file to begin.
        /// </summary>
        [BuiltinFunction("reset")]
        protected void reset(IList<IValue> Args, out IValue result)
        {
            result = new None();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
        }
    }
}
