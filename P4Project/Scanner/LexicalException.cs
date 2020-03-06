using System;
using System.Collections.Generic;
using System.Text;

namespace ScannerLib
{
    public class LexicalException : Exception
    {
        public LexicalException(int line, int symbol) : base($"Lexical error on line:{line} with symbol {symbol}")
        {
        }
    }
}
