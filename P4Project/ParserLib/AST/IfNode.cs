using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IfNode : StmtNode
    {
        public ExpressionNode ControlExpression;
        public List<ElifNode> ElifNodes;
        public ElseNode ElseNode;
        public BlockNode IfBody;
    }
}
