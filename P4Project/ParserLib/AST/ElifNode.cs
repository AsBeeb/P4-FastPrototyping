using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ElifNode : StmtNode
    {
        public ExpressionNode ControlExpr;
        public BlockNode ElifBody;
    }
}
