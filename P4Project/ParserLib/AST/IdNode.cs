using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class IdNode : ASTnode
    {
        public string Id;
        public List<IdOperationNode> IdOperations;

        public IdNode(string id, List<IdOperationNode> idOperations)
        {
            Id = id;
            IdOperations = idOperations;
        }
    }
}
