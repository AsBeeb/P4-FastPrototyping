using ParserLib.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib
{
    public abstract class Visitor
    {
        public void Visit(ASTnode ast)
        {
            ast.Accept(this);
        }

        protected abstract void Visit(ArrayAccessNode node);
        protected abstract void Visit(AssignmentNode node);
        protected abstract void Visit(BinaryExpressionNode node);
        protected abstract void Visit(BlockNode node);
        protected abstract void Visit(BoolValueNode node);
        protected abstract void Visit(ConstructorNode node);
        protected abstract void Visit(DeclarationNode node);
        protected abstract void Visit(ElifNode node);
        protected abstract void Visit(ElseNode node);
        protected abstract void Visit(FieldAccessNode node);
        protected abstract void Visit(FloatValueNode node);
        protected abstract void Visit(FormalParamNode node);
        protected abstract void Visit(FuncCallExpressionNode node);
        protected abstract void Visit(FuncCallStmtNode node);
        protected abstract void Visit(FunctionDclNode node);
        protected abstract void Visit(GlobalDclNode node);
        protected abstract void Visit(IdExpressionNode node);
        protected abstract void Visit(IdNode node);
        protected abstract void Visit(IfNode node);
        protected abstract void Visit(IntValueNode node);
        protected abstract void Visit(PlayLoopNode node);
        protected abstract void Visit(ProgNode node);
        protected abstract void Visit(ReturnNode node);
        protected abstract void Visit(StringValueNode node);
        protected abstract void Visit(StructDclNode node);
        protected abstract void Visit(UnaryExpressionNode node);
        protected abstract void Visit(WhileNode node);
    }
}
