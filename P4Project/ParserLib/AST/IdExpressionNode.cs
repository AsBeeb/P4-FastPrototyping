using ParserLib.AST.DataStructures;
using System.Collections.Generic;

namespace ParserLib.AST
{
    public class IdExpressionNode : ExpressionNode, IIdentifier
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
            Type = type;
        }
    }
}
