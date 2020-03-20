using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class AssignmentNode : StmtNode
    {
        public IdNode LeftValue;
        public ExpressionNode RightValue;
    }
}
