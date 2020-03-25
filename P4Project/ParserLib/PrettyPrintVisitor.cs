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
