using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScannerLib
{
    class CharReader : StreamReader
    {
        public CharReader(string InputFile) : base(InputFile)
        {
        }

        new public char Peek()
        {
            return (char) base.Peek();
        }

        public char Advance()
        {
            return (char)base.Read();
        }
    }
}
