using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ConstructorNode : ASTnode
    {
        public IdNode Id;
        public List<FormalParamNode> FormalParamNodes;
        public BlockNode Block;

        public ConstructorNode(IdNode id, List<FormalParamNode> formalParamNodes, BlockNode block)
        {
            Id = id;
            FormalParamNodes = formalParamNodes;
            Block = block;
        }
    }
}
