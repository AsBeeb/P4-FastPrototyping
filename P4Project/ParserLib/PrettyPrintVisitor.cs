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

        private void PrettyPrintNewLine()
        {
            Console.Write($"\n " + new string(' ', 4 * IndentationLevel));
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
            Console.Write($"{node.IntValue}");
        }

        internal override void Visit(PlayLoopNode node)
        {
            node.Player.Accept(this);
            Console.Write(" vs ");
            node.Opponent.Accept(this);
            Console.Write(" in ");
            node.AllPlayers.Accept(this);
            IndentationLevel++;
            PrettyPrintNewLine();
            node.PlayLoopBody.Accept(this);
            IndentationLevel--;
            PrettyPrintNewLine();
            Console.Write(" until");
            node.UntilCondition.Accept(this);
        }

        internal override void Visit(ProgNode node)
        {
            foreach (TopDclNode DclNode in node.TopDclNodes)
            {
                DclNode.Accept(this);
            }
        }

        internal override void Visit(ReturnNode node)
        {
            node.ReturnValue.Accept(this);
        }

        internal override void Visit(StringValueNode node)
        {
            Console.Write(node.StringValue);
        }

        internal override void Visit(StructDclNode node)
        {
            node.Id.Accept(this);
            foreach (DeclarationNode DclNode in node.Declarations)
            {
                DclNode.Accept(this);
            }
            node.Constructor.Accept(this);
        }

        internal override void Visit(UnaryExpressionNode node)
        {
            UnaryOperator Operator = node.Operator;

            switch (Operator)
            {
                case UnaryOperator.DEFAULT:
                    Console.Write("");
                    break;
                case UnaryOperator.NOT:
                    Console.Write(" !");
                    break;
                case UnaryOperator.UNARY_MINUS:
                    Console.Write(" -");
                    break;
                default:
                    break;
            }

            node.ExprNode.Accept(this);
            
        }

        internal override void Visit(WhileNode node)
        {
            node.ControlExpr.Accept(this);
            node.WhileLoopBody.Accept(this);
        }
    }
}
