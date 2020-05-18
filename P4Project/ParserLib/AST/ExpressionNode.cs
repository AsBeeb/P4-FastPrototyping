namespace ParserLib.AST
{
    public abstract class ExpressionNode : ASTnode
    {
        public string Type;

        public ExpressionNode() { }

        public ExpressionNode(string type)
        {
            Type = type;
        }
    }
}
