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

        public IfNode(ExpressionNode ControlExpression, BlockNode IfBody, List<ElifNode> ElifNodes, ElseNode ElseNode)
        {
            this.ControlExpression = ControlExpression;
            this.IfBody = IfBody;
            this.ElifNodes = ElifNodes;
            this.ElseNode = ElseNode;
        }
        public void Accept(Visitor v)
        {
            v.visit(this);
        }
    }
}
