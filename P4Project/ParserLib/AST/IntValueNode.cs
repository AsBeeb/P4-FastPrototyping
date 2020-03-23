using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IntValueNode : ExpressionNode
    {
        public int IntValue;
        public IntValueNode(int IntValue) : base("Int")
        {
            this.IntValue = IntValue;
        }
        public void Accept(Visitor v)
        {
            v.visit(this);
        }
    }
}
