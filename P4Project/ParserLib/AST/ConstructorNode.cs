using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ConstructorNode : ASTnode
    {
        public IdNode Id;
        public List<FormalParamNode> FormalParamNodes;
        public List<StmtNode> StmtNodes;
    }
}
