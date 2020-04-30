using ParserLib.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SemanticLib
{
    public class SymbolTable
    {
        public Dictionary<string, ASTnode> GlobalScope;
        public Stack<Dictionary<string, ASTnode>> Scopes;

        public SymbolTable()
        {
            Scopes = new Stack<Dictionary<string, ASTnode>>();
            Scopes.Push(new Dictionary<string, ASTnode>());
            GlobalScope = Scopes.Peek();            
        }

        public void OpenScope()
        {
            Scopes.Push(new Dictionary<string, ASTnode>());
        }

        public void CloseScope()
        {
            Scopes.Pop();
        }

        public void EnterSymbol(string symbolName, ASTnode astnode)
        {
            if (!IsDeclaredLocally(symbolName))
            {
                Scopes.Peek().Add(symbolName, astnode);
            }
            else
            {
                throw new SemanticException($"Error on line {astnode.line}: Symbol {symbolName} already declared locally.");
            }
        }

        public ASTnode RetrieveSymbol(string symbolName, ASTnode problemNode = null)
        {
            ASTnode returnValue;
            foreach (Dictionary<string, ASTnode> scope in Scopes)
            {
                if (scope.TryGetValue(symbolName, out returnValue))
                {
                    return returnValue;
                }
            }
            if (problemNode != null)
            {
                throw new SemanticException($"Error on line {problemNode.line}: Symbol {symbolName} not found. Potentially missing declaration or not visible in scope.");
            }
            else
            {
                throw new SemanticException($"Symbol {symbolName} not found. Potentially missing declaration or not visible in scope.");
            }
        }

        public bool IsDeclaredLocally(string symbolToFind)
        {
            return Scopes.Peek().ContainsKey(symbolToFind);
        }
    }
}
