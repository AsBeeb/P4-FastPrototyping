﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public enum BinaryOperator
    {
        GREATER_OR_EQUALS, LESS_OR_EQUALS, GREATER_THAN, LESS_THAN, EQUALS, NOT_EQUALS,
        PLUS, MINUS, MULTIPLY, DIVIDE, MODULO, OR, AND, STRING_CONCAT, POWER
    }
    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode LeftExpr;
        public ExpressionNode RightExpr;
        public BinaryOperator Operator;
    }
}
