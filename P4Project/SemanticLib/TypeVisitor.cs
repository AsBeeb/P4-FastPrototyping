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
        SymbolTable symbolTable;
        public TypeVisitor(SymbolTable symbolTable)
        {
            this.symbolTable = symbolTable;
        }
        public override void Visit(ArrayAccessNode node)
        {
            node.IndexValue.Accept(this);
            if (node.IndexValue.Type != "int")
            {
                throw new Exception("ArrayIndex is not of type integer (not cool).");
            }
        }

        public override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            node.RightValue.Accept(this);
            CompatibleTypes(node.LeftValue.Type, node.RightValue.Type, "assignment");
        }

        public override void Visit(BinaryExpressionNode node)
        {
            node.LeftExpr.Accept(this);
            node.RightExpr.Accept(this);
            CompareBinaryTypes(node);
        }

        public override void Visit(BlockNode node)
        {
            node.StmtNodes?.ForEach(x => x.Accept(this));
        }

        public override void Visit(BoolValueNode node)
        {
            node.Type = "bool";
        }

        public override void Visit(ConstructorNode node)
        {
            symbolTable.EnterScope();
            node.FormalParamNodes?.ForEach(x => x.Accept(this));
            node.Block.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(DeclarationNode node)
        {
            node.InitialValue?.Accept(this);
            string declarationType = node.Type + (node.IsArray ? "[]" : "");
            if (node.InitialValue != null)
            {
                CompatibleTypes(declarationType, node.InitialValue.Type, "initializer");
            }
        }

        public override void Visit(ElifNode node)
        {
            symbolTable.EnterScope();
            node.ControlExpr.Accept(this);

            if (node.ControlExpr.Type != "bool")
            {
                throw new Exception($"Elif control expression expected type bool, was {node.ControlExpr.Type}.");
            }

            node.ElifBody.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(ElseNode node)
        {
            symbolTable.EnterScope();
            node.ElseBody.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(FieldAccessNode node)
        {
            return;
        }

        public override void Visit(FloatValueNode node)
        {
            node.Type = "float";
        }

        public override void Visit(FormalParamNode node)
        {
            return;
        }

        public override void Visit(FuncCallExpressionNode node)
        {
            node.ActualParameters?.ForEach(x => x.Accept(this));
            var astNode = symbolTable.RetrieveSymbol(node.Id.Id);
            if (astNode is FunctionDclNode funcDcl)
            {
                if (node.ActualParameters.Count == funcDcl.FormalParamNodes.Count)
                {
                    for (int i = 0; i < node.ActualParameters.Count; i++)
                    {
                        CompatibleTypes(node.ActualParameters[i].Type, funcDcl.FormalParamNodes[i].Type, "parameter type");
                    }
                }
                else
                {
                    throw new Exception($"No function of name {node.Id.Id} with {node.ActualParameters.Count} found.");
                }

                node.Type = funcDcl.ReturnType;
            }
            else
            {
                throw new Exception($"Function with id: {node.Id.Id} not found.");
            }
        }

        public override void Visit(FuncCallStmtNode node)
        {
            node.ActualParameters?.ForEach(x => x.Accept(this));
            var astNode = symbolTable.RetrieveSymbol(node.Id.Id);
            if (astNode is FunctionDclNode funcDcl)
            {
                if (node.ActualParameters.Count == funcDcl.FormalParamNodes.Count)
                {
                    for (int i = 0; i < node.ActualParameters.Count; i++)
                    {
                        CompatibleTypes(node.ActualParameters[i].Type, funcDcl.FormalParamNodes[i].Type, "parameter type");
                    }
                }
                else
                {
                    throw new Exception($"No function of name {node.Id.Id} with {node.ActualParameters.Count} found.");
                }
            }
            else
            {
                throw new Exception($"Function with id: {node.Id.Id} not found.");
            }
        }

        public override void Visit(FunctionDclNode node)
        {
            string returnNodeType;
            symbolTable.EnterScope();
            node.FuncBody.Accept(this);
            List<ReturnNode> returnNodes = node.FuncBody.StmtNodes.Where(x => x is ReturnNode).Select(x => (ReturnNode)x).ToList();
            foreach (ReturnNode rNode in returnNodes)
            {
                returnNodeType = (rNode.ReturnValue != null) ? rNode.ReturnValue.Type : "void";
                if (returnNodeType != node.ReturnType)
                {
                    throw new Exception($"Return type invalid. Expected {node.ReturnType}, found {returnNodeType}.");
                }
            }

            symbolTable.CloseScope();
        }

        public override void Visit(GlobalDclNode node)
        {
            node.InitialValue.Accept(this);
            string declarationType = node.Type + (node.IsArray ? "[]" : "");
            CompatibleTypes(declarationType, node.InitialValue.Type, "initializer");
        }

        public override void Visit(IdExpressionNode node)
        {
            VisitIdNode(node);
        }

        public override void Visit(IdNode node)
        {
            VisitIdNode(node);
        }

        public override void Visit(IfNode node)
        {
            symbolTable.EnterScope();
            node.ControlExpression.Accept(this);
            if (node.ControlExpression.Type != "bool")
            {
                throw new Exception($"If control expression expected type bool, was {node.ControlExpression.Type}.");
            }
            node.IfBody.Accept(this);
            symbolTable.CloseScope();
            node.ElifNodes?.ForEach(x => x.Accept(this));
            node.ElseNode?.Accept(this);
        }

        public override void Visit(IntValueNode node)
        {
            node.Type = "int";
        }

        public override void Visit(PlayLoopNode node)
        {
            symbolTable.EnterScope();
            node.AllPlayers.Accept(this);
            node.UntilCondition.Accept(this);
            if (!node.AllPlayers.Type.Contains("[]")){
                throw new Exception("PlayLoop expected array.");
            }
            if (node.UntilCondition.Type != "bool")
            {
                throw new Exception("Expected boolean expression in the until condition.");
            }
            node.Player.Type = node.AllPlayers.Type.Replace("[]", "");
            node.Opponents.Type = node.AllPlayers.Type;
            node.PlayLoopBody.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(ProgNode node)
        {
            node.TopDclNodes.ForEach(x => x.Accept(this));
        }

        public override void Visit(ReturnNode node)
        {
            node.ReturnValue.Accept(this);
        }

        public override void Visit(StringValueNode node)
        {
            node.Type = "string";
        }

        public override void Visit(StructDclNode node)
        {
            symbolTable.EnterScope();
            node.Declarations?.ForEach(x => x.Accept(this));
            node.Constructor?.Accept(this);
            symbolTable.CloseScope();
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
                        throw new Exception($"Invalid type: {node.ExprNode.Type}. Expected type bool.");
                    }
                        break;
                case UnaryOperator.UNARY_MINUS:
                    if (node.ExprNode.Type == "int"|| node.ExprNode.Type == "float")
                    {
                        node.Type = node.ExprNode.Type;
                    }
                    else
                    {
                        throw new Exception($"Invalid type {node.ExprNode.Type}. Expected int or float.");
                    }
                    break;
                default:
                    throw new Exception($"Invalid unary operator.");
            }
        }

        public override void Visit(WhileNode node)
        {
            symbolTable.EnterScope();
            node.ControlExpr.Accept(this);
            if (node.ControlExpr.Type != "bool")
            {
                throw new Exception($"Expected boolean expression, found {node.ControlExpr.Type}.");
            }
            node.WhileLoopBody.Accept(this);
            symbolTable.CloseScope();
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
                            throw new Exception($"Invalid binary operation (int plus {node.RightExpr.Type} isn't legal).");
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
                            throw new Exception($"Invalid binary operation (int plus {node.RightExpr.Type} isn't legal).");
                        }
                    }
                    break;
                case BinaryOperator.DIVIDE:
                    if (node.LeftExpr.Type == "int" || node.LeftExpr.Type == "float")
                    {
                        if (node.RightExpr.Type == "int" || node.RightExpr.Type == "float")
                        {
                            node.Type = "float";
                        }
                        else
                        {
                            throw new Exception($"Invalid binary operation (division with type {node.RightExpr.Type} isn't legal).");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid binary operation (division with type {node.LeftExpr.Type} isn't legal).");
                    }
                    break;
                case BinaryOperator.MODULO:
                    if (node.LeftExpr.Type == "int" || node.LeftExpr.Type == "float")
                    {
                        if (node.RightExpr.Type == "int")
                        {
                            node.Type = node.LeftExpr.Type;
                        }
                        else
                        {
                            throw new Exception($"Invalid binary operation (modulo with type {node.RightExpr.Type} isn't legal).");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid binary operation (modulo with type {node.LeftExpr.Type} isn't legal).");
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
                            throw new Exception($"Invalid binary operation (comparison with  {node.RightExpr.Type} isn't legal).");
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
                            throw new Exception($"Invalid binary operation (string concatenation with {node.RightExpr.Type} isn't legal).");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid binary operation (string concatenation with {node.LeftExpr.Type} isn't legal).");
                    }
                    break;
                default:
                    throw new Exception($"Invalid binary operator.");
            }
        }

        public void CompatibleTypes(string firstType, string secondType, string exceptionString)
        {
            if (firstType != secondType)
            {
                switch (firstType)
                {
                    case "int":
                        if (secondType != "float")
                        {
                            throw new Exception($"Invalid {exceptionString} (can't convert {secondType} to type int).");
                        }
                        break;
                    case "float":
                        if (secondType != "int")
                        {
                            throw new Exception($"Invalid {exceptionString} (can't convert {secondType} to type float).");
                        }
                        break;
                    case "string":
                        if (secondType != "float" && secondType != "int" && secondType != "bool")
                        {
                            throw new Exception($"Invalid {exceptionString} (can't convert {secondType} to type string).");
                        }
                        break;
                    default:
                        throw new Exception($"Invalid {exceptionString} (can't convert {secondType} to type {firstType}).");
                }
            }
        }

        private void VisitIdNode(INode node)
        {
            ASTnode rootNode = symbolTable.RetrieveSymbol(node.GetId);
            if (rootNode != null)
            {
                ASTnode previousNode = rootNode;
                bool tempIsArray = true;
                if (node.GetIdOperations == null)
                {
                    if (rootNode is IDeclaration iDcl) // 448-455 NYT.
                    {
                        node.SetType(iDcl.GetDclType + (iDcl.GetIsArray? "[]" : ""));
                    }
                    else
                    {
                        throw new Exception("EEEEH");
                    }
                    return;
                }
                    

                foreach (IdOperationNode idOp in node.GetIdOperations)
                {
                    // FieldOperation
                    if (idOp is FieldAccessNode field)
                    {
                        // DeclaratioNode and GlobalDclNode
                        if (previousNode is IDeclaration dclNode)
                        {
                            if (tempIsArray && dclNode.GetIsArray)
                            {
                                throw new Exception("Illegal field reference on array.");
                            }

                            ASTnode tempNode = symbolTable.RetrieveSymbol(dclNode.GetDclType);
                            if (tempNode is StructDclNode structDcl)
                            {
                                DeclarationNode tempDclNode = structDcl.Declarations.FirstOrDefault(x => x.Id.Id == field.Id.Id);
                                if (tempDclNode != null)
                                {
                                    previousNode = tempDclNode;
                                    tempIsArray = tempDclNode.GetIsArray;
                                }
                                else
                                {
                                    throw new Exception($"Struct: {structDcl.Id.Id} is missing the field: {field.Id.Id}.");
                                }
                            }
                            else
                            {
                                throw new Exception("Undeclared struct access.");
                            }
                        }
                        else
                        {
                            throw new Exception("Previousnode wasn't IDeclaration type");
                        }
                    }
                    // ArrayOperation
                    else if (idOp is ArrayAccessNode array)
                    {
                        // Prevent two-dimensional arrays
                        int idOpIndex = node.GetIdOperations.IndexOf(idOp);
                        if (idOpIndex > 0)
                        {
                            IdOperationNode previousIdOp = node.GetIdOperations[idOpIndex - 1];
                            if (previousIdOp is ArrayAccessNode)
                            {
                                throw new Exception("Illegal two-dimensional array.");
                            }
                        }

                        if (previousNode is IDeclaration dcl)
                        {
                            if (dcl.GetIsArray)
                            {
                                tempIsArray = false;
                            }
                        }
                        else
                        {
                            throw new Exception("ID was not an declared as array.");
                        }
                    }

                }

                if (previousNode is IDeclaration iDecl) // 526-533 NYT.
                {
                    node.SetType(iDecl.GetDclType + (iDecl.GetIsArray && tempIsArray ? "[]" : ""));
                }
                else
                {
                    throw new Exception("EEEEH2");
                }

            }

        }

    }
}
