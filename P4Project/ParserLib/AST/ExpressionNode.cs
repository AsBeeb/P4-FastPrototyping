using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ExpressionNode : ASTnode
    {
        public string Type;

        public ExpressionNode(string type)
        {
            Type = type;
        }
    }
}
