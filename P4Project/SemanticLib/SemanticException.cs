using System;

namespace SemanticLib
{
    public class SemanticException : Exception
    {
        public SemanticException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}
