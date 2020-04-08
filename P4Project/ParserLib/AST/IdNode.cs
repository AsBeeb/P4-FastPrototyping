﻿using ParserLib.AST.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IdNode : ASTnode, INode, IDeclaration
    {
        public string Id;
        public List<IdOperationNode> IdOperations;
        public string Type;

        public string GetId => Id;
        public List<IdOperationNode> GetIdOperations => IdOperations;

        public bool GetIsArray => Type.Contains("[]");

        public string GetDclType => Type.Replace("[]", "");

        public IdNode(string id, List<IdOperationNode> idOperations)
        {
            Id = id;
            IdOperations = idOperations;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }

        public void SetType(string type)
        {
            Console.WriteLine("Set " + Id + " type to: " + type);
            Type = type;
        }
    }
}
