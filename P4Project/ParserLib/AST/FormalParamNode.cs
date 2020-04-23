using System;
using System.Collections.Generic;
using System.Text;
using ParserLib.AST.DataStructures;

namespace ParserLib.AST
{
    public class FormalParamNode : ASTnode, IVariableBinding
    {
        public IdNode Id;
        public string Type;
        public bool IsArray;

        public FormalParamNode(IdNode id, string type, bool isArray = false)
        {
            Id = id;
            Type = type;
            IsArray = isArray;
        }

        public bool GetIsArray => IsArray;

        public string GetVarType => Type;

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
