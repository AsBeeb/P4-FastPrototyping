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
        public Scope CurrentScope;
        public Scope GlobalScope;

        public SymbolTable()
        {
            GlobalScope = new Scope();
            CurrentScope = GlobalScope;
            
        }
        public void NewScope ()
        {
            Scope newScope = new Scope(CurrentScope);
            CurrentScope.Children.Add(newScope);
            CurrentScope = newScope;
        }

        public void EnterScope()
        {
            if (CurrentScope.Children.Count > 0 && CurrentScope.Children[CurrentScope.NextVisitedChild] != null)
            {
                CurrentScope = CurrentScope.Children[CurrentScope.NextVisitedChild++];
            }
            else
            {
                throw new Exception("Ran out of scopes");
            }
        }
        public void CloseScope()
        {
            CurrentScope.NextVisitedChild = 0;
            CurrentScope = CurrentScope.Parent;
        }

        public void EnterSymbol(string symbolName, ASTnode astnode)
        {
            if (!IsDeclaredLocally(symbolName))
            {
                CurrentScope.Symbols.Add(symbolName, astnode);
            }
            else
            {
                throw new Exception("Symbol already declared");
            }
        }

        public ASTnode RetrieveSymbol(string symbolName)
        {
            ASTnode returnValue;
            Scope viewingScope = CurrentScope;
            do
            {
                if (viewingScope.Symbols.TryGetValue(symbolName, out returnValue))
                {
                    return returnValue;
                }
                viewingScope = viewingScope.Parent;
            }
            while (viewingScope != null);
            throw new Exception("Symbol not found");
        }

        public bool IsDeclaredLocally(string symbolToFind)
        {
            return CurrentScope.Symbols.ContainsKey(symbolToFind);
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
                Console.WriteLine("Level: " + level + " - Name: " + kp.Key + " - Type: " + kp.Value.GetType());
            }
        }
    }
}
