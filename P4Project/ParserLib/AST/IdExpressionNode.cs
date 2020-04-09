using ParserLib.AST.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IdExpressionNode : ExpressionNode, INode
    {
        public string Id;
        public List<IdOperationNode> IdOperations;

        public string GetId => Id;
        public List<IdOperationNode> GetIdOperations => IdOperations;


        public IdExpressionNode(string id, List<IdOperationNode> operations)
        {
            Id = id;
            IdOperations = operations;
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
