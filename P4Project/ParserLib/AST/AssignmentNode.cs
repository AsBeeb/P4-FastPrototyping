using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class AssignmentNode : StmtNode
    {
        public IdNode LeftValue;
        public ExpressionNode RightValue;

        public AssignmentNode(IdNode leftValue, ExpressionNode rightValue)
        {
            LeftValue = leftValue;
            RightValue = rightValue;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }

    }
}
