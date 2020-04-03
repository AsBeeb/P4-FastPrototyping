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
        public char Name;
        public Scope Parent;
        public List<Scope> Children = new List<Scope>();
        public Dictionary<string, ASTnode> Symbols = new Dictionary<string, ASTnode>();

        public Scope(char name, Scope parent = null)
        {
            Name = name;
            Parent = parent;
        }
    }
}
