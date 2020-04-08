using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ProgNode : ASTnode
    {
        public List<TopDclNode> TopDclNodes;

        public ProgNode(List<TopDclNode> TopDclNodes)
        {
            this.TopDclNodes = TopDclNodes;   
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
