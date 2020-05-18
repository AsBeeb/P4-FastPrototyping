using System.Collections.Generic;

namespace ParserLib.AST
{
    public class ProgNode : ASTnode
    {
        public List<TopDclNode> TopDclNodes;

        public ProgNode(List<TopDclNode> topDclNodes)
        {
            this.TopDclNodes = topDclNodes;   
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
