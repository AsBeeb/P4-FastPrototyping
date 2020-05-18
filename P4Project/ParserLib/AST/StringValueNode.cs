namespace ParserLib.AST
{
    public class StringValueNode : ExpressionNode
    {
        public string StringValue;

        public StringValueNode(string val) : base("string")
        {
            StringValue = val;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
