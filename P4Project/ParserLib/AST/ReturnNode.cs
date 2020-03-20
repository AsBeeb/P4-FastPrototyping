using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ReturnNode : StmtNode
    {
        public ExpressionNode ReturnValue;
        public ReturnNode(ExpressionNode ReturnValue)
        {
            this.ReturnValue = ReturnValue;
        }
    }
}
