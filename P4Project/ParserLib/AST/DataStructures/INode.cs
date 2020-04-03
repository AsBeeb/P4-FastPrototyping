using System.Collections.Generic;

namespace ParserLib.AST.DataStructures
{
    public interface INode
    {
        string GetId { get; }
        List<IdOperationNode> GetIdOperations { get; }
    }
}
