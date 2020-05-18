using System.Collections.Generic;

namespace ParserLib.AST
{
    public class FuncCallStmtNode : StmtNode
    {
        public IdNode Id;
        public List<ExpressionNode> ActualParameters;

        public FuncCallStmtNode(IdNode id, List<ExpressionNode> actualParameters)
        {
            Id = id;
            ActualParameters = actualParameters;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
