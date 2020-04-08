using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FormalParamNode : ASTnode
    {
        public IdNode Id;
        public string Type;

        public FormalParamNode(IdNode id, string type)
        {
            Id = id;
            Type = type;
        }
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
