using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ExpressionNode : ASTnode
    {
        public string Type;

        public ExpressionNode()
        {

        }

        public ExpressionNode(string type)
        {
            Type = type;
        }
    }
}
