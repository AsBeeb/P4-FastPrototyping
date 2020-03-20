using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FuncCallStmtNode : StmtNode
    {
        public IdNode Id;
        public List<ExpressionNode> ActualParameters;
    }
}
