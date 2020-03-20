using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ArrayAccessNode : IdOperationNode
    {
        public ExpressionNode IndexValue;
    }
}
