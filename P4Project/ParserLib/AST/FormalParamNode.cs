﻿using System;
using System.Collections.Generic;
using System.Text;
using ParserLib.AST.DataStructures;

namespace ParserLib.AST
{
    public class FormalParamNode : ASTnode, IDeclaration
    {
        public IdNode Id;
        public string Type;

        public FormalParamNode(IdNode id, string type)
        {
            Id = id;
            Type = type;
        }

        public bool GetIsArray => false;

        public string GetDclType => Type;

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
