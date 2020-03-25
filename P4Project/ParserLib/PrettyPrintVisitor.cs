using ParserLib.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib
{
    public class PrettyPrintVisitor : Visitor
    {
        private int IndentationLevel = 0;

        private void PrettyPrint(string str)
        {
            Console.Write(new string(' ', 4 * IndentationLevel) + str);
        }

        internal override void Visit(ArrayAccessNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(AssignmentNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(BinaryExpressionNode node)
        {
            node.LeftExpr.Accept(this);
            //Print rigtig operator evt. med switch/dic
            node.RightExpr.Accept(this);
        }

        internal override void Visit(BlockNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(BoolValueNode node)
        {
            PrettyPrint(node.BoolValue.ToString());
        }

        internal override void Visit(ConstructorNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(DeclarationNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(ElifNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(ElseNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(ExpressionNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(FieldAccessNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(FloatValueNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(FormalParamNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(FuncCallExpressionNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(FuncCallStmtNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(FunctionDclNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(GlobalDclNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(IdExpressionNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(IdNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(IdOperationNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(IfNode node)
        {
            IndentationLevel++;
            node.IfBody.Accept(this);
            IndentationLevel--;
        }

        internal override void Visit(IntValueNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(PlayLoopNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(ProgNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(StmtNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(StringValueNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(StructDclNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(TopDclNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(UnaryExpressionNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(WhileNode node)
        {
            throw new NotImplementedException();
        }
    }
}
