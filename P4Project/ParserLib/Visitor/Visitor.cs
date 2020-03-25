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

        internal abstract void Visit(ArrayAccessNode node);
        internal abstract void Visit(AssignmentNode node);
        internal abstract void Visit(BinaryExpressionNode node);
        internal abstract void Visit(BlockNode node);
        internal abstract void Visit(BoolValueNode node);
        internal abstract void Visit(ConstructorNode node);
        internal abstract void Visit(DeclarationNode node);
        internal abstract void Visit(ElifNode node);
        internal abstract void Visit(ElseNode node);
        internal abstract void Visit(FieldAccessNode node);
        internal abstract void Visit(FloatValueNode node);
        internal abstract void Visit(FormalParamNode node);
        internal abstract void Visit(FuncCallExpressionNode node);
        internal abstract void Visit(FuncCallStmtNode node);
        internal abstract void Visit(FunctionDclNode node);
        internal abstract void Visit(GlobalDclNode node);
        internal abstract void Visit(IdExpressionNode node);
        internal abstract void Visit(IdNode node);
        internal abstract void Visit(IfNode node);
        internal abstract void Visit(IntValueNode node);
        internal abstract void Visit(PlayLoopNode node);
        internal abstract void Visit(ProgNode node);
        internal abstract void Visit(ReturnNode node);
        internal abstract void Visit(StringValueNode node);
        internal abstract void Visit(StructDclNode node);
        internal abstract void Visit(UnaryExpressionNode node);
        internal abstract void Visit(WhileNode node);
    }
}
