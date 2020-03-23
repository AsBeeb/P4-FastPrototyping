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
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
