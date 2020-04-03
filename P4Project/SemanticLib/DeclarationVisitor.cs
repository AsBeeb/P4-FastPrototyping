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
    public class DeclarationVisitor : Visitor
    {
        private SymbolTable symbolTable;

        public DeclarationVisitor(SymbolTable table)
        {
            symbolTable = table;
        }

        protected override void Visit(ArrayAccessNode node)
        {
            node.IndexValue.Accept(this);
        }

        protected override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            node.RightValue.Accept(this);
        }

        protected override void Visit(BinaryExpressionNode node)
        {
            node.LeftExpr.Accept(this);
            node.RightExpr.Accept(this);
        }

        protected override void Visit(BlockNode node)
        {
            foreach (StmtNode stmtNode in node.StmtNodes)
            {
                stmtNode.Accept(this);
            }
        }

        protected override void Visit(BoolValueNode node)
        {
            //skal være tom (true false)
        }

        protected override void Visit(ConstructorNode node)
        {
            symbolTable.OpenScope();
            foreach (FormalParamNode formalParam in node.FormalParamNodes)
            {
                formalParam.Accept(this);
            }
            node.Block.Accept(this);
            symbolTable.CloseScope();
        }

        protected override void Visit(DeclarationNode node)
        {
            symbolTable.EnterSymbol(node.Id.Id, node);
            node.InitialValue.Accept(this);
        }

        protected override void Visit(ElifNode node)
        {
            symbolTable.OpenScope();
            node.ControlExpr.Accept(this);
            node.ElifBody.Accept(this);
            symbolTable.CloseScope();
        }

        protected override void Visit(ElseNode node)
        {
            symbolTable.OpenScope();
            node.ElseBody.Accept(this);
            symbolTable.CloseScope();
        }

        protected override void Visit(FieldAccessNode node)
        {
            node.Id.Accept(this);
        }

        protected override void Visit(FloatValueNode node)
        {
            // skal være tom (float værdi)
        }

        protected override void Visit(FormalParamNode node)
        {
            symbolTable.EnterSymbol(node.Id.Id, node);
        }

        protected override void Visit(FuncCallExpressionNode node)
        {
            foreach (ExpressionNode parameter in node.ActualParameters)
            {
                parameter.Accept(this);
            }
            node.Id.Accept(this);
        }

        protected override void Visit(FuncCallStmtNode node)
        {
            foreach (ExpressionNode parameter in node.ActualParameters)
            {
                parameter.Accept(this);
            }
            node.Id.Accept(this);
        }

        protected override void Visit(FunctionDclNode node)
        {
            if (!(symbolTable.GlobalScope.Symbols.ContainsKey(node.ReturnType) || node.ReturnType == "int" || node.ReturnType == "float" || node.ReturnType == "bool" || node.ReturnType == "string" || node.ReturnType == "void"))
            {
                throw new Exception("Type doesn't exist");
            }

            symbolTable.OpenScope();
            foreach (FormalParamNode param in node.FormalParamNodes)
            {
                param.Accept(this);
            }
            node.FuncBody.Accept(this);

            symbolTable.CloseScope();
        }

        protected override void Visit(GlobalDclNode node)
        {
            node.InitialValue?.Accept(this);
        }

        protected override void Visit(IdExpressionNode node)
        {
            ASTnode rootNode = symbolTable.RetrieveSymbol(node.Id);
            if (rootNode != null)
            {
                ASTnode previousNode = rootNode;
                bool tempIsArray = true;

                foreach (IdOperationNode idOp in node.IdOperations)
                {
                    // FieldOperation
                    if (idOp is FieldAccessNode field)
                    {
                        // DeclaratioNode and GlobalDclNode
                        if(previousNode is IDeclaration dclNode)
                        {
                            if (tempIsArray && dclNode.GetIsArray)
                            {
                                // Error
                                return;
                            }

                            ASTnode tempNode = symbolTable.RetrieveSymbol(dclNode.GetDclType);
                            if(tempNode is StructDclNode structDcl)
                            {
                                DeclarationNode tempDclNode = structDcl.Declarations.FirstOrDefault(x => x.Id.Id == field.Id.Id);
                                if(tempDclNode != null)
                                {
                                    previousNode = tempDclNode;
                                    tempIsArray = tempDclNode.GetIsArray;
                                }
                            }
                            else
                            {
                                // Error
                                return;
                            }
                        }
                    }
                    // ArrayOperation
                    else if (idOp is ArrayAccessNode array)
                    {
                        // Prevent two-dimensional arrays
                        int idOpIndex = node.IdOperations.IndexOf(idOp);
                        if(idOpIndex > 0)
                        {
                            IdOperationNode previousIdOp = node.IdOperations[idOpIndex - 1];
                            if(previousIdOp is ArrayAccessNode)
                            {
                                // Error
                                return;
                            }
                        }

                        if (previousNode is IDeclaration dcl)
                        {
                            if (dcl.GetIsArray)
                            {
                                tempIsArray = false;
                            }
                            {
                                // Error
                                return;
                            }
                        }
                        else
                        {
                            // Error
                            return;
                        }
                    }
                }
            }
        }

        protected override void Visit(IdNode node)
        {
            throw new NotImplementedException();
        }

        protected override void Visit(IfNode node)
        {
            throw new NotImplementedException();
        }

        protected override void Visit(IntValueNode node)
        {
            throw new NotImplementedException();
        }

        protected override void Visit(PlayLoopNode node)
        {
            throw new NotImplementedException();
        }

        protected override void Visit(ProgNode node)
        {
            foreach (TopDclNode topDclNode in node.TopDclNodes)
            {
                switch (topDclNode)
                {
                    case StructDclNode structDcl:
                        symbolTable.EnterSymbol(structDcl.Id.Id, structDcl);
                        break;
                    case FunctionDclNode funcDcl:
                        symbolTable.EnterSymbol(funcDcl.Id.Id, funcDcl);
                        break;
                    case GlobalDclNode globalDcl:
                        symbolTable.EnterSymbol(globalDcl.Id.Id, globalDcl);
                        break;
                }
            }
            node.TopDclNodes.ForEach(x => x.Accept(this));
        }

        protected override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
        }

        protected override void Visit(StringValueNode node)
        {
            throw new NotImplementedException();
        }

        protected override void Visit(StructDclNode node)
        {
            throw new NotImplementedException();
        }

        protected override void Visit(UnaryExpressionNode node)
        {
            throw new NotImplementedException();
        }

        protected override void Visit(WhileNode node)
        {
            throw new NotImplementedException();
        }
    }
}
