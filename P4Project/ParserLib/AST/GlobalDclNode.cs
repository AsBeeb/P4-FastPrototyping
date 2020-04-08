using ParserLib.AST.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class GlobalDclNode : TopDclNode, IDeclaration
    {
        public IdNode Id;
        public ExpressionNode InitialValue;
        public bool IsArray;
        public string Type;

        public IdNode GetId => Id;
        public bool GetIsArray => IsArray;
        public string GetDclType => Type;

        public GlobalDclNode(IdNode id, ExpressionNode initVal, string type, bool isArray)
        {
            Id = id;
            InitialValue = initVal;
            Type = type;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
