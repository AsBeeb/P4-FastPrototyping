namespace ParserLib.AST
{
    public class BoolValueNode : ExpressionNode
    {
        public bool BoolValue;

        public BoolValueNode(bool boolValue) : base("bool")
        {
            BoolValue = boolValue;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
