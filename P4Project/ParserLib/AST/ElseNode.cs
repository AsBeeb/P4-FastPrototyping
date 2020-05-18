namespace ParserLib.AST
{
    public class ElseNode : StmtNode
    {
        public BlockNode ElseBody;

        public ElseNode(BlockNode elseBody)
        {
            ElseBody = elseBody;
        }

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
