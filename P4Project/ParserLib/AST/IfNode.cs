using System.Collections.Generic;

namespace ParserLib.AST
{
    public class IfNode : StmtNode
    {
        public ExpressionNode ControlExpression;
        public BlockNode IfBody;
        public List<ElifNode> ElifNodes;
        public ElseNode ElseNode;

        public IfNode(ExpressionNode controlExpression, BlockNode ifBody, List<ElifNode> elifNodes, ElseNode elseNode)
        {
            ControlExpression = controlExpression;
            IfBody = ifBody;
            ElifNodes = elifNodes;
            ElseNode = elseNode;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
