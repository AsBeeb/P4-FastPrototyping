namespace ParserLib.AST
{
    public class IntValueNode : ExpressionNode
    {
        public int IntValue;

        public IntValueNode(int intValue) : base("int")
        {
            this.IntValue = intValue;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
