using System;
using System.Collections.Generic;
using ScannerLib;
using System.Text;

namespace ParserLib
{
    class SyntacticalException : Exception
    {
        public SyntacticalException(Token problemToken): base($"Unexpected {problemToken.Type} at line {problemToken.Line}")
        {
        }
    }
}
