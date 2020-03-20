﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class FunctionDclNode : TopDclNode
    {
        public IdNode Id;
        public string ReturnType;
        public List<FormalParamNode> FormalParamNodes;
        public BlockNode FuncBody;

        public FunctionDclNode(IdNode id, string returnType, List<FormalParamNode> formalParams, BlockNode body)
        {
            Id = id;
            ReturnType = returnType;
            FormalParamNodes = formalParams;
            FuncBody = body;
        }
    }
}