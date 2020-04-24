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
                throw new SemanticException($"Error on line {node.line}: ArrayIndex is not of type integer.");
            }
        }

        public override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            node.RightValue.Accept(this);
            CompatibleTypes(node.LeftValue.Type, node.RightValue.Type, "assignment", node);
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
                CompatibleTypes(declarationType, node.InitialValue.Type, "initializer", node);
            }
        }

        public override void Visit(ElifNode node)
        {
            symbolTable.EnterScope();
            node.ControlExpr.Accept(this);

            if (node.ControlExpr.Type != "bool")
            {
                throw new SemanticException($"Error on line {node.line}: Elif control expression expected type bool, was {node.ControlExpr.Type}.");
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
            var astNode = symbolTable.RetrieveSymbol(node.Id.Id, node);
            if (astNode is FunctionDclNode funcDcl)
            {
                if (funcDcl.Id.Id == "ChooseOption")
                {
                    if (node.ActualParameters.Count > 1)
                    {
                        for (int i = 1; i < node.ActualParameters.Count; i++)
                        {
                            if (node.ActualParameters[i].Type != "string")
                            {
                                throw new SemanticException($"Error on line {node.line}: Wrong type {node.ActualParameters[i].Type} provided for ChooseOption.");
                            }
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: The ChooseOption did not receive enough parameters.");
                    }
                }
                else if (funcDcl.Id.Id == "Print")
                {
                    if (node.ActualParameters.Count == 1)
                    {
                        if (!IsPrimitiveType(node.ActualParameters[0].Type))
                        {
                            throw new SemanticException($"Error on line {node.line}: The Print function can only use integers, floats, booleans and strings.");
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: No function of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                    }
                }
                else if (funcDcl.Id.Id == "ListRemove" || funcDcl.Id.Id == "ListAdd")
                {
                    if (node.ActualParameters.Count == 2)
                    {
                        if (!(node.ActualParameters[0].Type.Contains("[]") && node.ActualParameters[1].Type == node.ActualParameters[0].Type.Replace("[]", "")))
                        {
                            throw new SemanticException($"Error on line {node.line}: Element type {node.ActualParameters[1].Type} was tried to be added or removed to array of type {node.ActualParameters[0].Type}.");
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: No function of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                    }
                }
                else if (funcDcl.Id.Id == "ListLength" || funcDcl.Id.Id == "ListEmpty")
                {
                    if (!(node.ActualParameters.Count == 1))
                    {
                        throw new SemanticException($"Error on line {node.line}: No function of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                    }
                    else
                    {
                        if (!node.ActualParameters[0].Type.Contains("[]"))
                        {
                            throw new SemanticException($"Error on line {node.line}: {node.Id.Id} expected list parameter.");
                        }
                    }
                }
                else if (node.ActualParameters.Count == funcDcl.FormalParamNodes.Count)
                {
                    for (int i = 0; i < node.ActualParameters.Count; i++)
                    {
                        //CompatibleTypes(node.ActualParameters[i].Type, funcDcl.FormalParamNodes[i].Type + (funcDcl.FormalParamNodes[i].IsArray? "[]" : "") , "parameter type", node);
                        if (node.ActualParameters[i].Type != funcDcl.FormalParamNodes[i].Type + (funcDcl.FormalParamNodes[i].IsArray ? "[]" : ""))
                        {
                            throw new SemanticException($"Error on line {node.line}: Invalid parameter type: can't convert {funcDcl.FormalParamNodes[i].Type + (funcDcl.FormalParamNodes[i].IsArray ? "[]" : "")} to type {node.ActualParameters[i].Type}.");
                        }
                    }
                }
                else
                {
                    throw new SemanticException($"Error on line {node.line}: No function of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                }

                node.Type = funcDcl.ReturnType;
            }
            else if (astNode is StructDclNode structDcl && structDcl.Constructor != null)
            {
                if (node.ActualParameters.Count == structDcl.Constructor.FormalParamNodes.Count)
                {
                    for (int i = 0; i < node.ActualParameters.Count; i++)
                    {
                        //CompatibleTypes(node.ActualParameters[i].Type, structDcl.Constructor.FormalParamNodes[i].Type + (structDcl.Constructor.FormalParamNodes[i].IsArray ? "[]" : ""), "parameter type", node);
                        if (node.ActualParameters[i].Type != structDcl.Constructor.FormalParamNodes[i].Type + (structDcl.Constructor.FormalParamNodes[i].IsArray ? "[]" : ""))
                        {
                            throw new SemanticException($"Error on line {node.line}: Invalid parameter type: can't convert {structDcl.Constructor.FormalParamNodes[i].Type + (structDcl.Constructor.FormalParamNodes[i].IsArray ? "[]" : "")} to type {node.ActualParameters[i].Type}.");
                        }
                    }
                }
                else
                {
                    throw new SemanticException($"Error on line {node.line}: No constructor of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                }

                node.Type = structDcl.Id.Id;
            }
            else
            {
                throw new SemanticException($"Error on line {node.line}: Function or constructor of name {node.Id.Id} not found.");
            }
        }

        public override void Visit(FuncCallStmtNode node)
        {
            node.ActualParameters?.ForEach(x => x.Accept(this));
            var astNode = symbolTable.RetrieveSymbol(node.Id.Id, node);
            if (astNode is FunctionDclNode funcDcl)
            {
                if (funcDcl.Id.Id == "ChooseOption")
                {
                    if (node.ActualParameters.Count > 1)
                    {
                        for (int i = 1; i < node.ActualParameters.Count; i++)
                        {
                            if (node.ActualParameters[i].Type != "string")
                            {
                                throw new SemanticException($"Error on line {node.line}: Wrong type {node.ActualParameters[i].Type} provided for ChooseOption.");
                            }
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: The ChooseOption did not receive enough parameters.");
                    }
                }
                else if (funcDcl.Id.Id == "Print")
                {
                    if (node.ActualParameters.Count == 1)
                    {
                        if (!IsPrimitiveType(node.ActualParameters[0].Type))
                        {
                            throw new SemanticException($"Error on line {node.line}: The Print function can only use integers, floats, booleans and strings.");
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: No function of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                    }
                }
                else if (funcDcl.Id.Id == "ListRemove" || funcDcl.Id.Id == "ListAdd")
                {
                    if (node.ActualParameters.Count == 2)
                    {
                        if (!(node.ActualParameters[0].Type.Contains("[]") && node.ActualParameters[1].Type == node.ActualParameters[0].Type.Replace("[]", "")))
                        {
                            throw new SemanticException($"Error on line {node.line}: Element type {node.ActualParameters[1].Type} was tried to be added or removed to array of type {node.ActualParameters[0].Type}.");
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: No function of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                    }
                }
                else if (funcDcl.Id.Id == "ListLength" || funcDcl.Id.Id == "ListEmpty")
                {
                    if (!(node.ActualParameters.Count == 1))
                    {
                        throw new SemanticException($"Error on line {node.line}: No function of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                    }
                    else
                    {
                        if (!node.ActualParameters[0].Type.Contains("[]"))
                        {
                            throw new SemanticException($"Error on line {node.line}: {node.Id.Id} expected list parameter.");
                        }
                    }
                }
                else if(node.ActualParameters.Count == funcDcl.FormalParamNodes.Count)
                {
                    for (int i = 0; i < node.ActualParameters.Count; i++)
                    {
                        //CompatibleTypes(node.ActualParameters[i].Type, funcDcl.FormalParamNodes[i].Type + (funcDcl.FormalParamNodes[i].IsArray? "[]": "" ), "parameter type", node);

                        if (node.ActualParameters[i].Type != funcDcl.FormalParamNodes[i].Type + (funcDcl.FormalParamNodes[i].IsArray ? "[]" : ""))
                        {
                            throw new SemanticException($"Error on line {node.line}: Invalid parameter type: can't convert {funcDcl.FormalParamNodes[i].Type + (funcDcl.FormalParamNodes[i].IsArray ? "[]" : "")} to type {node.ActualParameters[i].Type}.");
                        }

                    }
                }
                else
                {
                    throw new SemanticException($"Error on line {node.line}: No function of name {node.Id.Id} with {node.ActualParameters.Count} parameters found.");
                }
            }
            else if (astNode is StructDclNode structDcl)
            {
                throw new SemanticException($"Error on line {node.line}: Invalid constructor for {node.Id.Id} used outside an expression context.");
            }
            else
            {
                throw new SemanticException($"Error on line {node.line}: Function with id: {node.Id.Id} not found.");
            }
        }

        public override void Visit(FunctionDclNode node)
        {
            string returnNodeType;
            symbolTable.EnterScope();
            node.FuncBody.Accept(this);
            List<ReturnNode> returnNodes = GetReturnNodes(node.FuncBody); //node.FuncBody.StmtNodes.Where(x => x is ReturnNode).Select(x => (ReturnNode)x).ToList();

            if (returnNodes.Count == 0 && node.ReturnType != "void")
            {
                throw new SemanticException($"Error on line {node.line}: Missing return statement. Expected return of type {node.ReturnType}.");
            }

            foreach (ReturnNode rNode in returnNodes)
            {
                returnNodeType = (rNode.ReturnValue != null) ? rNode.ReturnValue.Type : "void";
                if (returnNodeType != node.ReturnType)
                {
                    throw new SemanticException($"Error on line {node.line}: Return type invalid. Expected {node.ReturnType}, found {returnNodeType}.");
                }
            }

            symbolTable.CloseScope();
        }

        public override void Visit(GlobalDclNode node)
        {
            node.InitialValue?.Accept(this);
            string declarationType = node.Type + (node.IsArray ? "[]" : "");
            if (node.InitialValue != null)
            {
                CompatibleTypes(declarationType, node.InitialValue.Type, "initializer", node);
            }
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
                throw new SemanticException($"Error on line {node.line}: If control expression expected type bool, was {node.ControlExpression.Type}.");
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
            if (!node.AllPlayers.Type.Contains("[]"))
            {
                throw new SemanticException($"Error on line {node.line}: PlayLoop expected array expression in loopheader, found {node.AllPlayers.Type}.");
            }
            if (node.UntilCondition.Type != "bool")
            {
                throw new SemanticException($"Error on line {node.line}: Expected boolean expression in the until condition, was {node.UntilCondition.Type}.");
            }
            node.Player.Type = node.AllPlayers.Type.Replace("[]", "");
            node.Opponents.Type = node.AllPlayers.Type;
            node.PlayLoopBody.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(ProgNode node)
        {
            node.TopDclNodes.ForEach(x => x.Accept(this));


            // Control for struct loops:
            List<StructDclNode> tempStructNodes = new List<StructDclNode>();

            foreach (TopDclNode itemNode in node.TopDclNodes)
            {
                if (itemNode is StructDclNode structDclNode)
                {
                    tempStructNodes.Add(structDclNode);
                }
            }

            foreach (StructDclNode structNodeItem in tempStructNodes)
            {
                List<string> structChain = new List<string>();

                CheckChildren(structNodeItem, structChain, symbolTable);

                if (structChain.Contains(structNodeItem.Id.Id))
                {
                    throw new SemanticException($"Error on line {node.line}: Error in struct {structNodeItem.Id.Id}. Possible loop or duplicate.");
                }
            }
        }

        public static void CheckChildren(StructDclNode node, List<string> liste, SymbolTable symbolTable)
        {
            foreach (DeclarationNode DCLNode in node.Declarations)
            {
                if (!IsPrimitiveType(DCLNode.Type))
                {
                    if (!DCLNode.Type.Contains("[]"))
                    {
                        if (!liste.Contains(DCLNode.Type))
                        {
                            liste.Add(DCLNode.Type);
                            StructDclNode STDCL = (StructDclNode)symbolTable.RetrieveSymbol(DCLNode.Type, DCLNode);
                            CheckChildren(STDCL, liste, symbolTable);
                        }
                    }
                }
            }
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
            //node.Declarations?.ForEach(x => x.Accept(this));
            foreach (DeclarationNode item in node.Declarations)
            {
                item.Accept(this);
                if (item.Type == node.Id.Id)
                {
                    throw new SemanticException($"Error on line {node.line}: Struct {node.Id.Id} cannot contain struct of type {item.Type}.");
                }
            }
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
                        throw new SemanticException($"Error on line {node.line}: Invalid type: {node.ExprNode.Type}. Expected type bool.");
                    }
                    break;
                case UnaryOperator.UNARY_MINUS:
                    if (node.ExprNode.Type == "int" || node.ExprNode.Type == "float")
                    {
                        node.Type = node.ExprNode.Type;
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: Invalid type {node.ExprNode.Type}. Expected int or float.");
                    }
                    break;
                default:
                    throw new SemanticException($"Error on line {node.line}: Invalid unary operator.");
            }
        }

        public override void Visit(WhileNode node)
        {
            symbolTable.EnterScope();
            node.ControlExpr.Accept(this);
            if (node.ControlExpr.Type != "bool")
            {
                throw new SemanticException($"Error on line {node.line}: Expected boolean expression, found {node.ControlExpr.Type}.");
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
                            throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"int + {node.RightExpr.Type}\".");
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
                            throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"float + {node.RightExpr.Type}\".");
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
                            throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} / {node.RightExpr.Type}\".");
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} / {node.RightExpr.Type}\".");
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
                            throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} % {node.RightExpr.Type}\".");
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} % {node.RightExpr.Type}\".");
                    }
                    break;
                case BinaryOperator.GREATER_OR_EQUALS:
                case BinaryOperator.GREATER_THAN:
                case BinaryOperator.LESS_OR_EQUALS:
                case BinaryOperator.LESS_THAN:
                    if (node.LeftExpr.Type == "int" || node.LeftExpr.Type == "float")
                    {
                        if (node.RightExpr.Type == "int" || node.RightExpr.Type == "float")
                        {
                            node.Type = "bool";
                        }
                        else
                        {
                            throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} {node.Operator} {node.RightExpr.Type}\".");
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} {node.Operator} {node.RightExpr.Type}\".");
                    }
                    break;
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
                            throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} {node.Operator} {node.RightExpr.Type}\".");
                        }
                    }
                    else if (node.LeftExpr.Type == "string" && node.RightExpr.Type == "string")
                    {
                        node.Type = "bool";
                    }
                    else if(node.LeftExpr.Type == "bool" && node.RightExpr.Type == "bool")
                    {
                        node.Type = "bool";
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} {node.Operator} {node.RightExpr.Type}\".");
                    }
                    break;
                case BinaryOperator.AND:
                case BinaryOperator.OR:
                    if (node.LeftExpr.Type == "bool" && node.RightExpr.Type == "bool")
                    {
                        node.Type = "bool";
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} {node.Operator} {node.RightExpr.Type}\".");
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
                            throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} : {node.RightExpr.Type}\".");
                        }
                    }
                    else
                    {
                        throw new SemanticException($"Error on line {node.line}: Invalid binary operation: \"{node.LeftExpr.Type} : {node.RightExpr.Type}\".");
                    }
                    break;
                default:
                    throw new SemanticException($"Error on line {node.line}: Invalid binary operator.");
            }
        }

        public void CompatibleTypes(string firstType, string secondType, string exceptionString, ASTnode problemNode)
        {
            if (firstType != secondType)
            {
                switch (firstType)
                {
                    case "int":
                        if (secondType != "float")
                        {
                            throw new SemanticException($"Error on line {problemNode.line}: Invalid {exceptionString}: can't convert {secondType} to type int.");
                        }
                        break;
                    case "float":
                        if (secondType != "int")
                        {
                            throw new SemanticException($"Error on line {problemNode.line}: Invalid {exceptionString}: can't convert {secondType} to type float.");
                        }
                        break;
                    case "string":
                        if (secondType != "float" && secondType != "int" && secondType != "bool")
                        {
                            throw new SemanticException($"Error on line {problemNode.line}: Invalid {exceptionString}: can't convert {secondType} to type string.");
                        }
                        break;
                    default:
                        throw new SemanticException($"Error on line {problemNode.line}: Invalid {exceptionString}: can't convert {secondType} to type {firstType}.");
                }
            }
        }

        private void VisitIdNode(IIdentifier node)
        {
            ASTnode rootNode = symbolTable.RetrieveSymbol(node.GetId);
            if (rootNode != null)
            {
                ASTnode previousNode = rootNode;
                bool tempIsArray = true;
                if (node.GetIdOperations == null)
                {
                    if (rootNode is IVariableBinding iDcl)
                    {
                        node.SetType(iDcl.GetVarType + (iDcl.GetIsArray ? "[]" : ""));
                    }
                    else
                    {
                        string funcOrStruct = (rootNode is FunctionDclNode) ? "function" : "struct";
                        throw new SemanticException($"Error on line {rootNode.line}: Expected variable identifier for {node.GetId}, was a {funcOrStruct} declaration.");
                    }
                    return;
                }


                foreach (IdOperationNode idOp in node.GetIdOperations)
                {
                    // FieldOperation
                    if (idOp is FieldAccessNode field)
                    {
                        // DeclaratioNode and GlobalDclNode
                        if (previousNode is IVariableBinding dclNode)
                        {
                            if (tempIsArray && dclNode.GetIsArray)
                            {
                                throw new SemanticException($"Error on line {rootNode.line}: Unexpected field reference in operations emanating from identifer {node.GetId}.");
                            }

                            ASTnode tempNode = symbolTable.RetrieveSymbol(dclNode.GetVarType, idOp);
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
                                    throw new SemanticException($"Error on line {rootNode.line}: Unexpected reference to field: {field.Id.Id} in struct: {structDcl.Id.Id}.");
                                }
                            }
                            else
                            {
                                throw new SemanticException($"Error on line {rootNode.line}: Accessing undeclared struct: {dclNode.GetVarType}.");
                            }
                        }
                        else
                        {
                            throw new SemanticException($"Error on line {rootNode.line}: Unexpected reference to non-variable identifier in operations emanating from {node.GetId}.");
                        }
                    }
                    // ArrayOperation
                    else if (idOp is ArrayAccessNode array)
                    {
                        // Type check for index expression in ArrayAccessNode.
                        array.Accept(this);

                        // Prevent two-dimensional arrays
                        int idOpIndex = node.GetIdOperations.IndexOf(idOp);
                        if (idOpIndex > 0)
                        {
                            IdOperationNode previousIdOp = node.GetIdOperations[idOpIndex - 1];
                            if (previousIdOp is ArrayAccessNode)
                            {
                                throw new SemanticException($"Error on line {rootNode.line}: Illegal reference to two-dimensional array in operations emanating from {node.GetId}.");
                            }
                        }

                        if (previousNode is IVariableBinding dcl)
                        {
                            if (dcl.GetIsArray)
                            {
                                tempIsArray = false;
                            }
                        }
                        else
                        {
                            throw new SemanticException($"Error on line {rootNode.line}: Unexpected reference to non-variable identifier in id operations emanating from {node.GetId}.");
                        }
                    }
                }

                if (previousNode is IVariableBinding iDecl)
                {
                    node.SetType(iDecl.GetVarType + (iDecl.GetIsArray && tempIsArray ? "[]" : ""));
                }
                else
                {
                    string funcOrStruct = (rootNode is FunctionDclNode) ? "function" : "struct";
                    throw new SemanticException($"Error on line {rootNode.line}: Unexpected reference to a {funcOrStruct} declaration in operations emanating from identifer {node.GetId}.");
                }

            }

        }
        public static bool IsPrimitiveType(string type)
        {
            bool isPrimitive = false;
            if (type == "bool" || type == "int" || type == "float" || type == "string")
            {
                isPrimitive = true;
            }
            return isPrimitive;
        }

        List<ReturnNode> GetReturnNodes(BlockNode block)
        {
            var returnList = new List<ReturnNode>();

            foreach (StmtNode stmt in block.StmtNodes)
            {
                if (stmt is ReturnNode rtrn)
                {
                    returnList.Add(rtrn);
                }
                else if (stmt is IfNode IfNde)
                {
                    returnList.AddRange(GetReturnNodes(IfNde.IfBody));
                    IfNde.ElifNodes?.ForEach(elifNode => returnList.AddRange(GetReturnNodes(elifNode.ElifBody)));
                    if (IfNde.ElseNode != null)
                    {
                        returnList.AddRange(GetReturnNodes(IfNde.ElseNode.ElseBody));
                    }
                }
                else if (stmt is WhileNode WhileNde)
                    {
                        returnList.AddRange(GetReturnNodes(WhileNde.WhileLoopBody));
                    }
                else if (stmt is PlayLoopNode playNode)
                {
                    returnList.AddRange(GetReturnNodes(playNode.PlayLoopBody));
                }
            }
            return returnList;
        }

    }
}
