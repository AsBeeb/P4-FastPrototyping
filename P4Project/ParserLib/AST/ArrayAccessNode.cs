using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ArrayAccessNode : IdOperationNode
    {
        public ExpressionNode IndexValue;

        public ArrayAccessNode(ExpressionNode indexValue)
        {
            IndexValue = indexValue;
        }

        public void Accept(Visitor v)
        {
            v.visit(this);
        }
    }
}
