using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public enum BinaryOperator
    {
        DEFAULT, GREATER_OR_EQUALS, LESS_OR_EQUALS, GREATER_THAN, LESS_THAN, EQUALS, NOT_EQUALS,
        PLUS, MINUS, MULTIPLY, DIVIDE, MODULO, OR, AND, STRING_CONCAT, POWER
    }
    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode LeftExpr;
        public ExpressionNode RightExpr;
        public BinaryOperator Operator;

        public BinaryExpressionNode(ExpressionNode leftExpr, ExpressionNode rightExpr, BinaryOperator op)
        {
            LeftExpr = leftExpr;
            RightExpr = rightExpr;
            Operator = op;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
