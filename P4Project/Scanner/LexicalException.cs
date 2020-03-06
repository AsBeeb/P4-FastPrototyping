using System;
using System.Collections.Generic;
using System.Text;

namespace ScannerLib
{
    public class LexicalException : Exception
    {
        public LexicalException(int line) : base($"Lexical error on line:{line}")
        {
        }
    }
}
