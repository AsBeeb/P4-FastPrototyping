namespace ParserLib.AST.DataStructures
{
    public interface IDeclaration
    {
        IdNode GetId { get; }
        bool GetIsArray { get; }
        string GetDclType { get; }
    }
}
