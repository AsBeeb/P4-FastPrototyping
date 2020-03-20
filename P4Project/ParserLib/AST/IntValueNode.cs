using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IntValueNode : ExpressionNode
    {
        public int IntValue;
        public IntValueNode(int IntValue)
        {
            this.IntValue = IntValue;
        }
    }
}
