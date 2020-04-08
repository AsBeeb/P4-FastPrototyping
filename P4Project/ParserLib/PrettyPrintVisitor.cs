using ParserLib.AST;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ParserLib
{
    public class PrettyPrintVisitor : Visitor
    {
        private int IndentationLevel = 0;


        private void PrettyPrintNewLine()
        {
            Console.Write($"\n" + new string(' ', 4 * IndentationLevel));
        }

        public override void Visit(ArrayAccessNode node)
        {
            Console.Write("[");
            node.IndexValue.Accept(this);
            Console.Write("]");
        }

        public override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            Console.Write(" = ");
            node.RightValue.Accept(this);
            Console.Write(";");
            PrettyPrintNewLine();
        }

        public override void Visit(BinaryExpressionNode node)
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

        public override void Visit(BlockNode node)
        {
            PrettyPrintNewLine();
            Console.Write("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            foreach (StmtNode stmt in node.StmtNodes)
            {
                stmt.Accept(this);
            }
            IndentationLevel--;
            PrettyPrintNewLine();
            Console.Write("}");
            PrettyPrintNewLine();
        }


        public override void Visit(BoolValueNode node)
        {
            Console.Write(node.BoolValue.ToString().ToLower());
        }

        public override void Visit(ConstructorNode node)
        {
            Console.Write(" ");
            node.Id.Accept(this);
            Console.Write(" (");
            node.FormalParamNodes?.ForEach(x => {
                if (node.FormalParamNodes.IndexOf(x) != 0)
                {
                    Console.Write(", ");
                }
                x.Accept(this);
            });
            Console.Write(")");
            node.Block.Accept(this);
        }

        public override void Visit(DeclarationNode node)
        {
            
            Console.Write("local " + node.Type.ToLower() + (node.IsArray ? "[] " : " "));
            
            node.Id.Accept(this);

            if (node.InitialValue != null)
            {
                Console.Write(" = ");
                node.InitialValue.Accept(this);
            }

            Console.Write(";");
            PrettyPrintNewLine();
        }

        public override void Visit(ElifNode node)
        {
            Console.Write("elif (");
            node.ControlExpr.Accept(this);
            Console.Write(")");
            node.ElifBody.Accept(this);
        }

        public override void Visit(ElseNode node)
        {
            Console.Write("else");
            node.ElseBody.Accept(this);
        }

        // De to Andreaser
        public override void Visit(FieldAccessNode node)
        {
            node.Id.Accept(this);
        }

        public override void Visit(FloatValueNode node)
        {
            Console.Write(node.FloatValue.ToString().Replace(",", "."));
        }

        public override void Visit(FormalParamNode node)
        {
            Console.Write(node.Type + " "); 
            node.Id.Accept(this);
        }

        public override void Visit(FuncCallExpressionNode node)
        {
            node.Id.Accept(this);
            Console.Write("(");
            node.ActualParameters?.ForEach(x => {
                if (node.ActualParameters.IndexOf(x) != 0)
                {
                    Console.Write(", ");
                }                   
                x.Accept(this);
            } );
            Console.Write(")");
        }

        public override void Visit(FuncCallStmtNode node)
        {
            node.Id.Accept(this);
            Console.Write("(");
            node.ActualParameters?.ForEach(x => {
                if (node.ActualParameters.IndexOf(x) != 0)
                {
                    Console.Write(", ");
                }               
                x.Accept(this);
            });
            Console.Write(")");
        }

        public override void Visit(FunctionDclNode node)
        {
            Console.Write("func " + node.ReturnType + " ");
            node.Id.Accept(this);
            Console.Write(" (");
            node.FormalParamNodes?.ForEach(x => {
                if (node.FormalParamNodes.IndexOf(x) != 0)
                {
                    Console.Write(", ");
                }                    
                x.Accept(this);
            });
            Console.Write(")");
            node.FuncBody.Accept(this);
        }

        public override void Visit(GlobalDclNode node)
        {
            Console.Write("global " + node.Type + " "); // global int 
            node.Id.Accept(this);
            Console.Write(" = ");
            node.InitialValue.Accept(this);
            Console.Write(";");
            PrettyPrintNewLine();
        }

        public override void Visit(IdExpressionNode node)
        {
            Console.Write(node.Id);
            node.IdOperations?.ForEach(x => {
                Console.Write(".");
                x.Accept(this); 
            });
        }

        public override void Visit(IdNode node)
        {
            Console.Write(node.Id);
            node.IdOperations?.ForEach(x => {
                Console.Write(".");
                x.Accept(this);
            });
        }

        public override void Visit(IfNode node)
        {
            Console.Write("if (");
            node.ControlExpression.Accept(this); 
            Console.Write(") ");
            node.IfBody.Accept(this);
            node.ElifNodes?.ForEach(x => x.Accept(this));
            node.ElseNode.Accept(this);
            Console.Write(";");
        }

        public override void Visit(IntValueNode node)
        {
            Console.Write($"{node.IntValue}");
        }

        public override void Visit(PlayLoopNode node)
        {
            Console.Write("play (");
            node.Player.Accept(this);
            Console.Write(" vs ");
            node.Opponents.Accept(this);
            Console.Write(" in ");
            node.AllPlayers.Accept(this);
            Console.Write(")");
            node.PlayLoopBody.Accept(this);
            Console.Write(" until (");
            node.UntilCondition.Accept(this);
            Console.Write(");");
        }

        public override void Visit(ProgNode node)
        {
            foreach (TopDclNode DclNode in node.TopDclNodes)
            {
                DclNode.Accept(this);
                PrettyPrintNewLine();
            }
        }

        public override void Visit(ReturnNode node)
        {
            Console.Write("return ");
            node.ReturnValue.Accept(this);
            Console.Write(";");
        }

        public override void Visit(StringValueNode node)
        {
            Console.Write($"\"{node.StringValue}\"" );
        }

        public override void Visit(StructDclNode node)
        {
            Console.Write("struct ");
            node.Id.Accept(this);
            Console.Write("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            foreach (DeclarationNode DclNode in node.Declarations)
            {
                DclNode.Accept(this);
            }
            node.Constructor.Accept(this);
            IndentationLevel--;
            PrettyPrintNewLine();
            Console.Write("}");
        }

        public override void Visit(UnaryExpressionNode node)
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

        public override void Visit(WhileNode node)
        {
            node.ControlExpr.Accept(this);
            node.WhileLoopBody.Accept(this);
        }
    }
}
