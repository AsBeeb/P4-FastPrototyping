using System.IO;

namespace ScannerLib
{
    public class CharReader : StreamReader
    {
        public CharReader(string InputFile) : base(InputFile)
        {
        }

        public new char Peek()
        {
            return (char) base.Peek();
        }

        public char Advance()
        {
            return (char)base.Read();
        }
    }
}
