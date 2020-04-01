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
        public void OpenScope ()
        {
            Scope newScope = new Scope(CurrentScope);
            CurrentScope.Children.Add(newScope);
            CurrentScope = newScope;
        }

        public void CloseScope()
        {
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
    }
}
