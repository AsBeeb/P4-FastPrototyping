using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ProgNode : ASTnode
    {
        public List<TopDclNode> TopDclNodes;

        public ProgNode(List<TopDclNode> nodes)
        {
            TopDclNodes = nodes;   
        }
    }
}
