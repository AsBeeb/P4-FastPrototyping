using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IfNode : StmtNode
    {
        public ExpressionNode ControlExpression;
        public BlockNode IfBody;
        public List<ElifNode> ElifNodes;
        public ElseNode ElseNode;

        public IfNode(ExpressionNode ControlExpression, BlockNode IfBody, List<ElifNode> ElifNodes, ElseNode ElseNode)
        {
            this.ControlExpression = ControlExpression;
            this.IfBody = IfBody;
            this.ElifNodes = ElifNodes;
            this.ElseNode = ElseNode;
        }
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
