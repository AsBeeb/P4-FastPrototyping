using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class BoolValueNode : ExpressionNode
    {
        public bool BoolValue;

        public BoolValueNode(bool boolValue) : base("Bool")
        {
            BoolValue = boolValue;
        }
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
