using ParserLib.AST.DataStructures;

namespace ParserLib.AST
{
    public class DeclarationNode : StmtNode, IVariableBinding
    {
        public IdNode Id;
        public string Type;
        public ExpressionNode InitialValue;
        public bool IsArray;

        public IdNode GetId => Id;
        public bool GetIsArray => IsArray;
        public string GetVarType => Type;

        public DeclarationNode(IdNode id, string type, ExpressionNode initialValue, bool isArray)
        {
            Id = id;
            Type = type;
            InitialValue = initialValue;
            IsArray = isArray;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
