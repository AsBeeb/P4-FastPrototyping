using ParserLib.AST.DataStructures;
using System.Collections.Generic;

namespace ParserLib.AST
{
    public class IdNode : ASTnode, IIdentifier, IVariableBinding
    {
        public string Id;
        public List<IdOperationNode> IdOperations;
        public string Type;

        public string GetId => Id;
        public List<IdOperationNode> GetIdOperations => IdOperations;

        public bool GetIsArray => Type.Contains("[]");
        public string GetVarType => Type.Replace("[]", "");

        public IdNode(string id, List<IdOperationNode> idOperations = null)
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
            Type = type;
        }
    }
}
