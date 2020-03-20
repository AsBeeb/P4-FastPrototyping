using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class WhileNode : StmtNode
    {
        public ExpressionNode ControlExpr;
        public BlockNode WhileLoopBody;
        public WhileNode(ExpressionNode ControlExpr, BlockNode WhileLoopBody)
        {
            this.ControlExpr = ControlExpr;
            this.WhileLoopBody = WhileLoopBody;
        }
    }
}
