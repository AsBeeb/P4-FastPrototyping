﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FuncCallExpressionNode : ExpressionNode
    {
        public IdNode Id;
        public List<ExpressionNode> ActualParameters;
    }
}
