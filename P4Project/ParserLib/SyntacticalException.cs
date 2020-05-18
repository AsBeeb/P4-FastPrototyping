using System;
using ScannerLib;

namespace ParserLib
{
    public class SyntacticalException : Exception
    {
        public SyntacticalException(Token problemToken): base($"Unexpected {problemToken.Type} at line {problemToken.Line}.")
        {
        }
    }
}
