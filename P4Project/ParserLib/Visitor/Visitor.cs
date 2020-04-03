using ParserLib.AST;

namespace ParserLib
{
    public abstract class Visitor
    {
        public void Visit(ASTnode ast)
        {
            ast.Accept(this);
        }

        public abstract void Visit(ArrayAccessNode node);
        public abstract void Visit(AssignmentNode node);
        public abstract void Visit(BinaryExpressionNode node);
        public abstract void Visit(BlockNode node);
        public abstract void Visit(BoolValueNode node);
        public abstract void Visit(ConstructorNode node);
        public abstract void Visit(DeclarationNode node);
        public abstract void Visit(ElifNode node);
        public abstract void Visit(ElseNode node);
        public abstract void Visit(FieldAccessNode node);
        public abstract void Visit(FloatValueNode node);
        public abstract void Visit(FormalParamNode node);
        public abstract void Visit(FuncCallExpressionNode node);
        public abstract void Visit(FuncCallStmtNode node);
        public abstract void Visit(FunctionDclNode node);
        public abstract void Visit(GlobalDclNode node);
        public abstract void Visit(IdExpressionNode node);
        public abstract void Visit(IdNode node);
        public abstract void Visit(IfNode node);
        public abstract void Visit(IntValueNode node);
        public abstract void Visit(PlayLoopNode node);
        public abstract void Visit(ProgNode node);
        public abstract void Visit(ReturnNode node);
        public abstract void Visit(StringValueNode node);
        public abstract void Visit(StructDclNode node);
        public abstract void Visit(UnaryExpressionNode node);
        public abstract void Visit(WhileNode node);
    }
}
