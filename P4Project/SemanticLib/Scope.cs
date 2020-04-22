using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLib;
using ParserLib.AST;

namespace SemanticLib
{
    public class Scope
    {
        public Scope Parent;
        public List<Scope> Children = new List<Scope>();
        public Dictionary<string, ASTnode> Symbols = new Dictionary<string, ASTnode>();

        public int NextVisitedChild = 0;

        public Scope(Scope parent = null)
        {
            Parent = parent;
        }
    }
}
