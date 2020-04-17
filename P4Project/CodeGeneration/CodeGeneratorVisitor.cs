﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLib;
using ParserLib.AST;

namespace CodeGeneration
{
    public class CodeGeneratorVisitor : Visitor
    {
        public StringBuilder CSharpString = new StringBuilder();
        private int IndentationLevel = 0;


        private void PrettyPrintNewLine()
        {
            CSharpString.Append($"\n" + new string(' ', 4 * IndentationLevel));
        }

        public override void Visit(ArrayAccessNode node)
        {
            CSharpString.Append("[");
            node.IndexValue.Accept(this);
            CSharpString.Append("]");
        }

        public override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            CSharpString.Append(" = ");
            node.RightValue.Accept(this);
            CSharpString.Append(";");
            PrettyPrintNewLine();
        }

        public override void Visit(BinaryExpressionNode node)
        {
            if (node.Operator != BinaryOperator.POWER && node.Operator != BinaryOperator.STRING_CONCAT)
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
                }
                CSharpString.Append(" " + binaryOperator + " ");
                node.RightExpr.Accept(this);
            }
            else
            {
                switch (node.Operator)
                {
                    case BinaryOperator.STRING_CONCAT:
                        CSharpString.Append("(");
                        node.LeftExpr.Accept(this);
                        CSharpString.Append(").ToString() + (");
                        node.RightExpr.Accept(this);
                        CSharpString.Append(").ToString()");
                        
                        break;
                    case BinaryOperator.POWER:
                        CSharpString.Append("Math.Pow(");
                        node.LeftExpr.Accept(this);
                        CSharpString.Append(",");
                        node.RightExpr.Accept(this);
                        CSharpString.Append(")");
                        break;
                }
            }
        }

        public override void Visit(BlockNode node)
        {
            PrettyPrintNewLine();
            CSharpString.Append("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            foreach (StmtNode stmt in node.StmtNodes)
            {
                stmt.Accept(this);
            }
            IndentationLevel--;
            PrettyPrintNewLine();
            CSharpString.Append("}");
            PrettyPrintNewLine();
        }


        public override void Visit(BoolValueNode node)
        {
            CSharpString.Append(node.BoolValue.ToString().ToLower());
        }

        public override void Visit(ConstructorNode node)
        {
            CSharpString.Append(" ");
            node.Id.Accept(this);
            CSharpString.Append(" (");
            node.FormalParamNodes?.ForEach(x => {
                if (node.FormalParamNodes.IndexOf(x) != 0)
                {
                    CSharpString.Append(", ");
                }
                x.Accept(this);
            });
            CSharpString.Append(")");
            node.Block.Accept(this);
        }

        public override void Visit(DeclarationNode node)
        {
            if (node.IsArray)
            {
                CSharpString.Append("List<" + node.Type + "> ");
            }
            else
            {
                CSharpString.Append(node.Type + " ");
            }
            node.Id.Accept(this);
            CSharpString.Append(" = ");

            if (node.InitialValue != null)
            {
                node.InitialValue.Accept(this);
            }
            else
            {
                //Default values if a variable isn't initialized
                if (node.IsArray)
                {
                    CSharpString.Append("new List<" + node.Type + ">()");
                }
                else
                {
                    switch (node.Type)
                    {
                        case "int":
                            CSharpString.Append("0");
                            break;
                        case "float":
                            CSharpString.Append("0.0f");
                            break;
                        case "string":
                            CSharpString.Append("\"\"");
                            break;
                        case "bool":
                            CSharpString.Append("false");
                            break;
                        default:
                            CSharpString.Append( "new " + node.Type + "()");
                            break;
                    }
                } 
            }
            CSharpString.Append(";");
            PrettyPrintNewLine();
        }

        public override void Visit(ElifNode node)
        {
            CSharpString.Append("else if (");
            node.ControlExpr.Accept(this);
            CSharpString.Append(")");
            node.ElifBody.Accept(this);
        }

        public override void Visit(ElseNode node)
        {
            CSharpString.Append("else");
            node.ElseBody.Accept(this);
        }

        public override void Visit(FieldAccessNode node)
        {
            CSharpString.Append(".");
            node.Id.Accept(this);
        }

        public override void Visit(FloatValueNode node)
        {
            CSharpString.Append(node.FloatValue.ToString().Replace(",", "."));
        }

        public override void Visit(FormalParamNode node)
        {
            CSharpString.Append(node.Type + " ");
            node.Id.Accept(this);
        }

        public override void Visit(FuncCallExpressionNode node)
        {
            node.Id.Accept(this);
            CSharpString.Append("(");
            node.ActualParameters?.ForEach(x => {
                if (node.ActualParameters.IndexOf(x) != 0)
                {
                    CSharpString.Append(", ");
                }
                x.Accept(this);
            });
            CSharpString.Append(")");
        }

        public override void Visit(FuncCallStmtNode node)
        {
            node.Id.Accept(this);
            CSharpString.Append("(");
            node.ActualParameters?.ForEach(x => {
                if (node.ActualParameters.IndexOf(x) != 0)
                {
                    CSharpString.Append(", ");
                }
                x.Accept(this);
            });
            CSharpString.Append(")");
        }

        public override void Visit(FunctionDclNode node)
        {
            CSharpString.Append("public ");
            // Checks if the function declared is main
            if (node.Id.Id == "main")
            {
                CSharpString.Append("static ");
                CSharpString.Append(node.ReturnType + " ");
                CSharpString.Append("Main");
            }
            else
            {
                CSharpString.Append(node.ReturnType + " ");
                node.Id.Accept(this);
            }
            CSharpString.Append("(");
            node.FormalParamNodes?.ForEach(x => {
                if (node.FormalParamNodes.IndexOf(x) != 0)
                {
                    CSharpString.Append(", ");
                }
                x.Accept(this);
            });
            CSharpString.Append(")");
            node.FuncBody.Accept(this);
        }

        public override void Visit(GlobalDclNode node)
        {
            CSharpString.Append(node.Type + " "); // global int example 
            node.Id.Accept(this);
            CSharpString.Append(" = ");
            node.InitialValue.Accept(this);
            CSharpString.Append(";");
            PrettyPrintNewLine();
        }

        public override void Visit(IdExpressionNode node)
        {
            CSharpString.Append(node.Id + "µ");
            node.IdOperations?.ForEach(x => {
                x.Accept(this);
            });
            
        }

        public override void Visit(IdNode node)
        {
            CSharpString.Append(node.Id + "µ");
            node.IdOperations?.ForEach(x => {
                x.Accept(this);
            });
        }

        public override void Visit(IfNode node)
        {
            CSharpString.Append("if (");
            node.ControlExpression.Accept(this);
            CSharpString.Append(") ");
            node.IfBody.Accept(this);
            node.ElifNodes?.ForEach(x => x.Accept(this));
            node.ElseNode?.Accept(this);
        }

        public override void Visit(IntValueNode node)
        {
            CSharpString.Append(node.IntValue.ToString());
        }

        public override void Visit(PlayLoopNode node)
        {
            // Vent til vi har programmeret dette.
            //Console.Write("play (");
            //node.Player.Accept(this);
            //Console.Write(" vs ");
            //node.Opponents.Accept(this);
            //Console.Write(" in ");
            //node.AllPlayers.Accept(this);
            //Console.Write(")");
            //node.PlayLoopBody.Accept(this);
            //Console.Write(" until (");
            //node.UntilCondition.Accept(this);
            //Console.Write(");");

            CSharpString.Append("while(!(");
            node.UntilCondition.Accept(this);
            CSharpString.Append("))");
            node.PlayLoopBody.Accept(this);
        }

        public override void Visit(ProgNode node)
        {
            CSharpString.Append("using System;");
            PrettyPrintNewLine();
            CSharpString.Append("using System.Collections.Generic;");
            PrettyPrintNewLine();
            CSharpString.Append("using System.Linq;");
            PrettyPrintNewLine();
            CSharpString.Append("using System.Text;");
            PrettyPrintNewLine();

            CSharpString.Append("public class Programµµ");
            PrettyPrintNewLine();
            CSharpString.Append("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            foreach (TopDclNode DclNode in node.TopDclNodes)
            {
                DclNode.Accept(this);
                PrettyPrintNewLine();
            }
            IndentationLevel--;
            PrettyPrintNewLine();
            CSharpString.Append("}");
        }

        public override void Visit(ReturnNode node)
        {
            CSharpString.Append("return ");
            node.ReturnValue.Accept(this);
            CSharpString.Append(";");
        }

        public override void Visit(StringValueNode node)
        {
            CSharpString.Append($"\"{node.StringValue}\"");
        }

        public override void Visit(StructDclNode node)
        {
            
            CSharpString.Append("class ");
            node.Id.Accept(this);
            CSharpString.Append("{");
            IndentationLevel++;
            PrettyPrintNewLine();
            foreach (DeclarationNode DclNode in node.Declarations)
            {
                DclNode.Accept(this);
            }

            if (node.Constructor == null || node.Constructor.FormalParamNodes.Count > 0)
            {
                CSharpString.Append("public " + node.Id.Id + "()");
                CSharpString.Append("{");
                PrettyPrintNewLine();
                CSharpString.Append("}");
                PrettyPrintNewLine();
            }

            node.Constructor.Accept(this);
            IndentationLevel--;
            PrettyPrintNewLine();
            CSharpString.Append("}");
        }

        public override void Visit(UnaryExpressionNode node)
        {
            UnaryOperator Operator = node.Operator;
            if (node.Operator != UnaryOperator.DEFAULT)
            {
                switch (Operator)
                {
                    case UnaryOperator.NOT:
                        CSharpString.Append(" !(");
                        break;
                    case UnaryOperator.UNARY_MINUS:
                        CSharpString.Append(" -(");
                        break;
                }
                node.ExprNode.Accept(this);
                CSharpString.Append(")");
            }
            else
            {
                node.ExprNode.Accept(this);
            }
        }

        public override void Visit(WhileNode node)
        {
            CSharpString.Append("while (");
            node.ControlExpr.Accept(this);
            CSharpString.Append(")");
            node.WhileLoopBody.Accept(this);
        }
    }
}
