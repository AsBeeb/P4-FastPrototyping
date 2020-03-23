using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class BlockNode : ASTnode
    {
        public List<StmtNode> StmtNodes;
        public BlockNode(List<StmtNode> stmtNodes)
        {
            StmtNodes = stmtNodes;
        }
        public void Accept(Visitor v)
        {
            v.visit(this);
        }

    }
}
