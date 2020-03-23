using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class ElseNode : StmtNode
    {
        public BlockNode ElseBody;

        public ElseNode(BlockNode elseBody)
        {
            ElseBody = elseBody;
        }
        public void Accept(Visitor v)
        {
            v.visit(this);
        }
    }
}
