﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FieldAccessNode : IdOperationNode
    {
        public IdNode Id;

        public FieldAccessNode(IdNode id)
        {
            Id = id;
        }
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
