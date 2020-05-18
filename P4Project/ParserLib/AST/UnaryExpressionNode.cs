namespace ParserLib.AST
{
    public enum UnaryOperator
    {
        DEFAULT, NOT, UNARY_MINUS
    }

    public class UnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode ExprNode;
        public UnaryOperator Operator;

        public UnaryExpressionNode(ExpressionNode exprNode, UnaryOperator unaryOperator)
        {
            this.ExprNode = exprNode;
            this.Operator = unaryOperator;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
