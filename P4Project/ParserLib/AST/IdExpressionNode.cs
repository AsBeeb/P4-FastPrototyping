﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IdExpressionNode : ExpressionNode
    {
        public string Id;
        public List<IdOperationNode> IdOperations;
    }
}
