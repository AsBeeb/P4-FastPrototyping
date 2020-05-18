namespace ParserLib.AST
{
    public class WhileNode : StmtNode
    {
        public ExpressionNode ControlExpr;
        public BlockNode WhileLoopBody;

        public WhileNode(ExpressionNode controlExpr, BlockNode whileLoopBody)
        {
            this.ControlExpr = controlExpr;
            this.WhileLoopBody = whileLoopBody;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
