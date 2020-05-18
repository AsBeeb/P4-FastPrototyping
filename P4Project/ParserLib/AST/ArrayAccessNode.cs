namespace ParserLib.AST
{
    public class ArrayAccessNode : IdOperationNode
    {
        public ExpressionNode IndexValue;

        public ArrayAccessNode(ExpressionNode indexValue)
        {
            IndexValue = indexValue;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
