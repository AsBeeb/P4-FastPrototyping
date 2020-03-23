using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class DeclarationNode : StmtNode
    {
        public IdNode Id;
        public string Type;
        public ExpressionNode InitialValue;
        public bool IsArray;

        public DeclarationNode(IdNode id, string type, ExpressionNode initialValue, bool isArray)
        {
            Id = id;
            Type = type;
            InitialValue = initialValue;
            IsArray = isArray;
        }
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
