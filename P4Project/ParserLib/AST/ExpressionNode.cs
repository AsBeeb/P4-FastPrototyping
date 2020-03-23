using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public abstract class ExpressionNode : ASTnode
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
