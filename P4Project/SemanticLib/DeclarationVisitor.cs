using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserLib;
using ParserLib.AST;

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
            symbolTable.EnterSymbol(node.Id.Id, node);
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
            symbolTable.EnterSymbol(node.Id.Id, node);
            
        }

        protected override void Visit(IdExpressionNode node)
        {
            ASTnode symbolnode = symbolTable.RetrieveSymbol(node.Id);
            if (symbolnode == null)
            {
                throw new Exception("Variable name not declared");
            }
            if (node.IdOperations != null)
            {
                bool isFieldReferenceLegalFlag = true;
                ASTnode AccessNode = symbolnode;

                foreach (IdOperationNode idOp in node.IdOperations)
                {
                    if (idOp is ArrayAccessNode arrayAccess)
                    {
                        if (AccessNode is DeclarationNode DCLNode)
                        {
                            if (DCLNode.IsArray)
                            {
                                bool isPrimitive = (DCLNode.Type == "int" || DCLNode.Type == "float" || DCLNode.Type == "bool" || DCLNode.Type == "string");
                                if (isPrimitive)
                                {
                                    isFieldReferenceLegalFlag = false;
                                }
                                else
                                {
                                    AccessNode = symbolTable.RetrieveSymbol(node.Type);
                                }
                            }

                        }
                    }
                    //else if (idOp is FieldAccessNode fieldaccess)
                    //{
                    //    if (!isFieldReferenceLegalFlag)
                    //    {
                    //        throw new Exception("Illegal field acces on a primitive type");
                    //    }
                    //    bool isPrimitive = (node.Type == "int" || node.Type == "float" || node.Type == "bool" || node.Type == "string");
                    //    if (isPrimitive)
                    //    {
                    //        isFieldReferenceLegalFlag = false;
                    //    }
                    //    else
                    //    {
                    //        AccessNode = symbolTable.RetrieveSymbol(node.Type);
                    //    }
                    //}
                    else
                    {
                        throw new Exception("This is a problem");
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
            throw new NotImplementedException();
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
