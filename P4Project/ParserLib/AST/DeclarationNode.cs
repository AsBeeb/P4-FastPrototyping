using ParserLib.AST.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class DeclarationNode : StmtNode, IDeclaration
    {
        public IdNode Id;
        public string Type;
        public ExpressionNode InitialValue;
        public bool IsArray;

        public IdNode GetId => Id;
        public bool GetIsArray => IsArray;
        public string GetDclType => Type;

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
