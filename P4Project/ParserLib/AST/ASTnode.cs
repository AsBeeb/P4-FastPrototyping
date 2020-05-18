namespace ParserLib.AST
{
    public abstract class ASTnode
    {
        public int Line;

        public abstract void Accept(Visitor v);
    }
}
