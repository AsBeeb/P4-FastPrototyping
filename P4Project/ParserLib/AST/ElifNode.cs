using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ElifNode : StmtNode
    {
        public ExpressionNode ControlExpr;
        public BlockNode ElifBody;

        public ElifNode(ExpressionNode controlExpr, BlockNode elifBody)
        {
            ControlExpr = controlExpr;
            ElifBody = elifBody;
        }
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
