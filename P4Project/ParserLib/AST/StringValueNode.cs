using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class StringValueNode : ExpressionNode
    {
        public string StringValue;

        public StringValueNode(string val) : base("String")
        {
            StringValue = val;
        }
    }
}
