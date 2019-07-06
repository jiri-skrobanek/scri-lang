using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    /// <summary>
    /// An environment for interfacing with a file.
    /// </summary>
    internal class FileEnvironment : Environment
    {
        private System.IO.FileStream stream;

        public FileEnvironment(System.IO.FileStream fileStream)
        {
            stream = fileStream;
            GlobalScope = new Scope(this)
            {
                Names = new Dictionary<string, IValue>()
                {
                    ["vector"] = new Buildin() { Call = getNewVector },
                    ["map"] = new Buildin() { Call = getNewMap },
                    ["char"] = new Buildin() { Call = makeChar }
                }
            };
            GlobalScope.Names.Add("reset", new Buildin { Call = reset });
            if (fileStream.CanRead)
            {
                GlobalScope.Names.Add("read", new Buildin { Call = getChar });
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
            if (b == -1)
            {
                result = new None();
            }
            else
            {
                result = new CharValue();
            }
        }

        /// <summary>
        /// Sets the position in the file to begin.
        /// </summary>
        private void reset(IList<IValue> Args, out IValue result)
        {
            result = new None();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
        }
    }
}
