using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class BlockNode : ASTnode
    {
        public List<StmtNode> StmtNodes;
    }
}
