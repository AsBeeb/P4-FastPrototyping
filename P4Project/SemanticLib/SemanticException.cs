using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticLib
{
    public class SemanticException : Exception
    {
        public SemanticException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}
