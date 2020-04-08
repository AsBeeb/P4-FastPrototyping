using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLib;
using ParserLib.AST;
using ParserLib.AST.DataStructures;

namespace SemanticLib
{
    public class TypeVisitor : Visitor
    {
        SymbolTable SymbolTable;
        public TypeVisitor(SymbolTable symbolTable)
        {
            SymbolTable = symbolTable;
        }
        public override void Visit(ArrayAccessNode node)
        {
            node.IndexValue.Accept(this);
            if (node.IndexValue.Type != "int")
            {
                throw new Exception("ArrayIndex is not of type integer (not cool)");
            }
        }

        public override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            node.RightValue.Accept(this);
            if(node.LeftValue.Type != node.RightValue.Type)
            {
                switch (node.LeftValue.Type)
                {
                    case "int":
                        if (node.RightValue.Type != "float")
                        {
                            throw new Exception($"Invalid assignment (can't convert {node.RightValue.Type} to type int)");
                        }
                        break;
                    case "float":
                        if (node.RightValue.Type != "int")
                        {
                            throw new Exception($"Invalid assignment (can't convert {node.RightValue.Type} to type float)");
                        }
                        break;
                    case "string":
                        if (node.RightValue.Type != "float" && node.RightValue.Type != "int" && node.RightValue.Type != "bool")
                        {
                            throw new Exception($"Invalid assignment (can't convert {node.RightValue.Type} to type string)");
                        }
                        break;
                    default:
                        throw new Exception($"Invalid assignment (can't convert {node.RightValue.Type} to type {node.LeftValue.Type})");
                }
            }
        }

        public override void Visit(BinaryExpressionNode node)
        {
            node.LeftExpr.Accept(this);
            node.RightExpr.Accept(this);
            CompareBinaryTypes(node);
        }

        private void CompareBinaryTypes(BinaryExpressionNode node)
        {
            switch (node.Operator)
            {
                case BinaryOperator.PLUS:
                case BinaryOperator.MINUS:
                case BinaryOperator.MULTIPLY:
                case BinaryOperator.POWER:
                    if (node.LeftExpr.Type == "int")
                    {
                        if (node.RightExpr.Type == "int")
                        {
                            node.Type = "int";
                        }
                        else if (node.RightExpr.Type == "float")
                        {
                            node.Type = "float";
                        }
                        else
                        {
                            throw new Exception($"Invalid binary operation (int plus {node.RightExpr.Type} isn't legal)");
                        }
                    }
                    else if (node.LeftExpr.Type == "float")
                    {
                        if (node.RightExpr.Type == "int" || node.RightExpr.Type == "float")
                        {
                            node.Type = "float";
                        }
                        else
                        {
                            throw new Exception($"Invalid binary operation (int plus {node.RightExpr.Type} isn't legal)");
                        }
                    }
                    break;
                case BinaryOperator.DIVIDE:
                    if (node.LeftExpr.Type == "int" ||node.LeftExpr.Type == "float")
                    {
                        if (node.RightExpr.Type == "int" || node.RightExpr.Type == "float")
                        {
                            node.Type = "float";
                        }
                        else
                        {
                            throw new Exception($"Invalid binary operation (division with type {node.RightExpr.Type} isn't legal)");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid binary operation (division with type {node.LeftExpr.Type} isn't legal)");
                    }
                    break;
                case BinaryOperator.MODULO:
                    if (node.LeftExpr.Type == "int"|| node.LeftExpr.Type == "float")
                    {
                        if (node.RightExpr.Type == "int")
                        {
                            node.Type = node.LeftExpr.Type;
                        }
                        else
                        {
                            throw new Exception($"Invalid binary operation (modulo with type {node.RightExpr.Type} isn't legal)");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid binary operation (modulo with type {node.LeftExpr.Type} isn't legal)");
                    }
                    break;
                case BinaryOperator.GREATER_OR_EQUALS:
                case BinaryOperator.GREATER_THAN:
                case BinaryOperator.LESS_OR_EQUALS:
                case BinaryOperator.LESS_THAN:
                case BinaryOperator.NOT_EQUALS:
                case BinaryOperator.EQUALS:
                    if (node.LeftExpr.Type == "int" || node.LeftExpr.Type == "float")
                    {
                        if (node.RightExpr.Type == "int" || node.RightExpr.Type == "float")
                        {
                            node.Type = "bool";
                        }
                        else
                        {
                            throw new Exception($"Invalid binary operation (comparison with  {node.RightExpr.Type} isn't legal)");
                        }
                    }
                    break;
                case BinaryOperator.AND:
                case BinaryOperator.OR:
                    if (node.LeftExpr.Type == "bool" && node.RightExpr.Type == "bool")
                    {
                        node.Type = "bool";
                    }
                    break;
                case BinaryOperator.STRING_CONCAT:
                    if (node.LeftExpr.Type == "bool" || node.LeftExpr.Type == "int" || node.LeftExpr.Type == "float" || node.LeftExpr.Type == "string")
                    {
                        if (node.RightExpr.Type == "bool" || node.RightExpr.Type == "int" || node.RightExpr.Type == "float" || node.RightExpr.Type == "string")
                        {
                            node.Type = "string";
                        }
                        else
                        {
                            throw new Exception($"Invalid binary operation (string concatenation with {node.RightExpr.Type} isn't legal)");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid binary operation (string concatenation with {node.LeftExpr.Type} isn't legal)");
                    }
                    break;
                default:
                    throw new Exception($"Invalid binary operator");
            }
        }

        public override void Visit(BlockNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BoolValueNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ConstructorNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DeclarationNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ElifNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ElseNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FieldAccessNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FloatValueNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FormalParamNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FuncCallExpressionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FuncCallStmtNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FunctionDclNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(GlobalDclNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IdExpressionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IdNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IfNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IntValueNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PlayLoopNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ProgNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(StringValueNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(StructDclNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(UnaryExpressionNode node)
        {
            node.ExprNode.Accept(this);
            switch (node.Operator)
            {
                case UnaryOperator.NOT:
                    if (node.ExprNode.Type == "bool")
                    {
                        node.Type = "bool";
                    }
                    else
                    {
                        throw new Exception($"Invalid type: {node.ExprNode.Type}. Expected type bool");
                    }
                        break;
                case UnaryOperator.UNARY_MINUS:
                    if (node.ExprNode.Type == "int"|| node.ExprNode.Type == "float")
                    {
                        node.Type = node.ExprNode.Type;
                    }
                    else
                    {
                        throw new Exception($"Invalid type {node.ExprNode.Type}. Expected int or float");
                    }
                    break;
                default:
                    throw new Exception($"Invalid unary operator");
            }
        }

        public override void Visit(WhileNode node)
        {
            throw new NotImplementedException();
        }
    }
}
