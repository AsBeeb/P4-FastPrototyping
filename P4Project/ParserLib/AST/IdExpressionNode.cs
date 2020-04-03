using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IdExpressionNode : ExpressionNode
    {
        public string Id;
        public List<IdOperationNode> IdOperations;

        public IdExpressionNode(string id, List<IdOperationNode> operations)
        {
            Id = id;
            IdOperations = operations;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
