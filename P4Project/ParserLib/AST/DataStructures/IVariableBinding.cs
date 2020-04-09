namespace ParserLib.AST.DataStructures
{
    public interface IVariableBinding
    {
        bool GetIsArray { get; }
        string GetVarType { get; }
    }
}
