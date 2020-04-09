using System.Collections.Generic;

namespace ParserLib.AST.DataStructures
{
    public interface IIdentifier
    {
        string GetId { get; }
        List<IdOperationNode> GetIdOperations { get; }

        void SetType(string type);
    }
}
