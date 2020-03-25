using ParserLib.AST;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.Write("[");
            node.IndexValue.Accept(this);
            Console.Write("]");
        }

        internal override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            Console.Write(" = ");
            node.RightValue.Accept(this);
        }

        internal override void Visit(BinaryExpressionNode node)
        {
            string binaryOperator = "";

            node.LeftExpr.Accept(this);
            
            switch (node.Operator)
            {
                case BinaryOperator.GREATER_OR_EQUALS:
                    binaryOperator = ">=";
                    break;
                case BinaryOperator.LESS_OR_EQUALS:
                    binaryOperator = "<=";
                    break;
                case BinaryOperator.GREATER_THAN:
                    binaryOperator = ">";
                    break;
                case BinaryOperator.LESS_THAN:
                    binaryOperator = "<";
                    break;
                case BinaryOperator.EQUALS:
                    binaryOperator = "==";
                    break;
                case BinaryOperator.NOT_EQUALS:
                    binaryOperator = "!=";
                    break;
                case BinaryOperator.PLUS:
                    binaryOperator = "+";
                    break;
                case BinaryOperator.MINUS:
                    binaryOperator = "-";
                    break;
                case BinaryOperator.MULTIPLY:
                    binaryOperator = "*";
                    break;
                case BinaryOperator.DIVIDE:
                    binaryOperator = "/";
                    break;
                case BinaryOperator.MODULO:
                    binaryOperator = "%";
                    break;
                case BinaryOperator.OR:
                    binaryOperator = "||";
                    break;
                case BinaryOperator.AND:
                    binaryOperator = "&&";
                    break;
                case BinaryOperator.STRING_CONCAT:
                    binaryOperator = ":";
                    break;
                case BinaryOperator.POWER:
                    binaryOperator = "^";
                    break;
            }
            Console.Write(" " + binaryOperator + " ");
            
            node.RightExpr.Accept(this);
        }

        internal override void Visit(BlockNode node)
        {
            Console.Write("\n{\n");
            foreach (StmtNode stmt in node.StmtNodes)
            {
                stmt.Accept(this);
            }
            Console.Write("}\n");
        }


        internal override void Visit(BoolValueNode node)
        {
            Console.Write(node.BoolValue);
        }

        internal override void Visit(ConstructorNode node)
        {
            node.Id.Accept(this);
            Console.Write(" (");

            int end = node.FormalParamNodes.Count - 1;

            for (int i = 0; i < end; i++)
            { 
                node.FormalParamNodes[i].Accept(this);
                Console.Write(", ");
            }

            node.FormalParamNodes.Last().Accept(this);
            Console.Write(")");

            node.Block.Accept(this);

        }

        internal override void Visit(DeclarationNode node)
        {
            Console.Write(node.Type + (node.IsArray ? "[] " : " "));
            
            node.Id.Accept(this);

            if (node.InitialValue != null)
            {
                Console.Write(" = ");
                node.InitialValue.Accept(this);
            }

            Console.WriteLine(";\n");
                
        }

        internal override void Visit(ElifNode node)
        {
            Console.Write("elif (");
            node.ControlExpr.Accept(this);
            Console.Write(")");
            node.ElifBody.Accept(this);
        }

        internal override void Visit(ElseNode node)
        {
            Console.Write("else");
            node.ElseBody.Accept(this);
        }

        // De to Andreaser
        internal override void Visit(FieldAccessNode node)
        {
            node.Id.Accept(this);
        }

        internal override void Visit(FloatValueNode node)
        {
            PrettyPrint(node.FloatValue.ToString());
        }

        internal override void Visit(FormalParamNode node)
        {
            node.Id.Accept(this);
            PrettyPrint(node.Type);
        }

        internal override void Visit(FuncCallExpressionNode node)
        {
            node.Id.Accept(this);
            node.ActualParameters.ForEach(x => x.Accept(this));
        }

        internal override void Visit(FuncCallStmtNode node)
        {
            node.Id.Accept(this);
            node.ActualParameters.ForEach(x => x.Accept(this));
        }

        internal override void Visit(FunctionDclNode node)
        {
            PrettyPrint("func " + node.ReturnType);
            node.Id.Accept(this);
            PrettyPrint(" (");
            node.FormalParamNodes.ForEach(x => x.Accept(this));
            PrettyPrint(") \n");
            node.FuncBody.Accept(this);
        }

        internal override void Visit(GlobalDclNode node)
        {
            PrettyPrint("global " + node.Type + " "); // global int 
            node.Id.Accept(this);
            PrettyPrint(" = ");
            node.InitialValue.Accept(this);
        }

        internal override void Visit(IdExpressionNode node)
        {
            PrettyPrint(node.Id);
            node.IdOperations.ForEach(x => {
                PrettyPrint(".");
                x.Accept(this); 
            });
        }

        internal override void Visit(IdNode node)
        {
            PrettyPrint(node.Id);
            node.IdOperations.ForEach(x => {
                PrettyPrint(".");
                x.Accept(this);
            });
        }

        internal override void Visit(IfNode node)
        {
            PrettyPrint("if (" + node.ControlExpression + ") \n");
            node.IfBody.Accept(this);
            node.ElifNodes.ForEach(x => x.Accept(this));
            node.ElseNode.Accept(this);
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
            node.PlayLoopBody.Accept(this);
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
