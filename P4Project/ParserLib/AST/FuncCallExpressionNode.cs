﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FuncCallExpressionNode : ExpressionNode
    {
        public IdNode Id;
        public List<ExpressionNode> ActualParameters;

        public FuncCallExpressionNode(IdNode id, List<ExpressionNode> actualParameters)
        {
            Id = id;
            ActualParameters = actualParameters;
        }
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
