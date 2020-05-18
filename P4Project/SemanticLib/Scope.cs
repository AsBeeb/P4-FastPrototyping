using System.Collections.Generic;
using ParserLib.AST;

namespace SemanticLib
{
    public class Scope
    {
        public Scope Parent;
        public List<Scope> Children = new List<Scope>();
        public Dictionary<string, ASTnode> Symbols = new Dictionary<string, ASTnode>();

        public Scope(Scope parent = null)
        {
            Parent = parent;
        }
    }
}
