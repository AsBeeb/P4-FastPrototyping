using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public abstract class ASTnode
    {
        public int line;
        public abstract void Accept(Visitor v);
    }
}
