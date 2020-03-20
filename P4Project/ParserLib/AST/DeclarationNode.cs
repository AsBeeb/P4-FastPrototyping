using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class DeclarationNode : StmtNode
    {
        public IdNode Id;
        public string Type;
        public ExpressionNode InitialValue;
    }
}
