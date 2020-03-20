using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public enum UnaryOperator
    {
        NOT, UNARY_MINUS
    }
    public class UnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode ExprNode;
        public UnaryOperator Operator;
        
    }
}
