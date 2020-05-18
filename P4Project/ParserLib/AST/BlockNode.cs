using System.Collections.Generic;

namespace ParserLib.AST
{
    public class BlockNode : ASTnode
    {
        public List<StmtNode> StmtNodes;

        public BlockNode(List<StmtNode> stmtNodes)
        {
            StmtNodes = stmtNodes;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
