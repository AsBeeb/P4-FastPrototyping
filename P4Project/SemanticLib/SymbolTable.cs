using ParserLib.AST;
using System;
using System.Collections.Generic;

namespace SemanticLib
{
    public class SymbolTable
    {
        public Scope GlobalScope;

        private Scope currentScope;

        public SymbolTable()
        {
            GlobalScope = new Scope();
            currentScope = GlobalScope;
        }

        public void OpenScope()
        {
            Scope newScope = new Scope(currentScope);
            currentScope.Children.Add(newScope);
            currentScope = newScope;
        }

        public void CloseScope()
        {
            currentScope = currentScope.Parent;
        }

        public void EnterSymbol(string symbolName, ASTnode astnode)
        {
            if (!IsDeclaredLocally(symbolName))
            {
                currentScope.Symbols.Add(symbolName, astnode);
            }
            else
            {
                throw new SemanticException($"Error on line {astnode.Line}: Symbol {symbolName} already declared locally.");
            }
        }

        public ASTnode RetrieveSymbol(string symbolName, ASTnode problemNode = null)
        {
            Scope viewingScope = currentScope;
            do
            {
                if (viewingScope.Symbols.TryGetValue(symbolName, out ASTnode returnValue))
                {
                    return returnValue;
                }
                viewingScope = viewingScope.Parent;
            }
            while (viewingScope != null);
            if (problemNode != null)
            {
                throw new SemanticException($"Error on line {problemNode.Line}: Symbol {symbolName} not found. Potentially missing declaration or not visible in scope.");
            }
            else
            {
                throw new SemanticException($"Symbol {symbolName} not found. Potentially missing declaration or not visible in scope.");
            }
        }

        public void PrintTable(Scope header, int level)
        {
            foreach(Scope scope in header.Children)
            {
                PrintTable(scope, level + 1);
            }

            foreach(KeyValuePair<string, ASTnode> kp in header.Symbols)
            {
                // Print
                Console.WriteLine("Level: " + level + " - Name: " + kp.Key + " - Type: " + kp.Value.GetType() + ".");
            }
        }

        private bool IsDeclaredLocally(string symbolToFind)
        {
            return currentScope.Symbols.ContainsKey(symbolToFind);
        }
    }
}
