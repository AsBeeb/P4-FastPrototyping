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

        public override void Visit(ArrayAccessNode node)
        {
            node.IndexValue.Accept(this);
        }

        public override void Visit(AssignmentNode node)
        {
            node.LeftValue.Accept(this);
            node.RightValue.Accept(this);
        }

        public override void Visit(BinaryExpressionNode node)
        {
            node.LeftExpr.Accept(this);
            node.RightExpr.Accept(this);
        }

        public override void Visit(BlockNode node)
        { 
            node.StmtNodes?.ForEach(x => x.Accept(this));
        }

        public override void Visit(BoolValueNode node)
        {
            //skal være tom (true false)
        }

        public override void Visit(ConstructorNode node)
        {
            symbolTable.NewScope();
            foreach (FormalParamNode formalParam in node.FormalParamNodes)
            {
                formalParam.Accept(this);
            }
            node.Block.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(DeclarationNode node)
        {
            if (node.Id.IdOperations?.Count > 0)
            {
                throw new Exception("Invalid identifier declaration.");

            }

            symbolTable.EnterSymbol(node.Id.Id, node);
            node.InitialValue?.Accept(this);
        }

        public override void Visit(ElifNode node)
        {
            symbolTable.NewScope();
            node.ControlExpr.Accept(this);
            node.ElifBody.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(ElseNode node)
        {
            symbolTable.NewScope();
            node.ElseBody.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(FieldAccessNode node)
        {
            node.Id.Accept(this);
        }

        public override void Visit(FloatValueNode node)
        {
            // skal være tom (float værdi)
        }

        public override void Visit(FormalParamNode node)
        {
            if (node.Id.IdOperations?.Count > 0)
            {
                throw new Exception("Invalid parameter declaration.");
            }

            symbolTable.EnterSymbol(node.Id.Id, node);
        }

        public override void Visit(FuncCallExpressionNode node)
        {
            node.ActualParameters?.ForEach(x => x.Accept(this));
            node.Id.Accept(this);
        }

        public override void Visit(FuncCallStmtNode node)
        {
            foreach (ExpressionNode parameter in node.ActualParameters)
            {
                parameter.Accept(this);
            }
            node.Id.Accept(this);
        }

        public override void Visit(FunctionDclNode node)
        {
            if (node.Id.IdOperations?.Count > 0)
            {
                throw new Exception("Invalid identifier declaration.");
            }

            if (!(symbolTable.GlobalScope.Symbols.ContainsKey(node.ReturnType) || node.ReturnType == "int" || node.ReturnType == "float" || node.ReturnType == "bool" || node.ReturnType == "string" || node.ReturnType == "void"))
            {
                throw new Exception("Type doesn't exist.");
            }

            symbolTable.NewScope();
            foreach (FormalParamNode param in node.FormalParamNodes)
            {
                param.Accept(this);
            }
            node.FuncBody.Accept(this);

            symbolTable.CloseScope();
        }

        public override void Visit(GlobalDclNode node)
        {
            if(node.Id.IdOperations?.Count > 0)
            {
                throw new Exception("Invalid identifier declaration.");
            }

            node.InitialValue?.Accept(this);
        }

        public override void Visit(IdExpressionNode node)
        {
            return;
        }

        public override void Visit(IdNode node)
        {
            return;
        }

        public override void Visit(IfNode node)
        {
            symbolTable.NewScope();
            node.ControlExpression.Accept(this);
            node.IfBody.Accept(this);
            symbolTable.CloseScope();
            node.ElifNodes?.ForEach(x => x.Accept(this));
            node.ElseNode?.Accept(this);
        }

        public override void Visit(IntValueNode node)
        {
            // Nothing
        }

        public override void Visit(PlayLoopNode node)
        {
            if (node.Player.IdOperations?.Count > 0 || node.Opponents.IdOperations?.Count > 0)
            {
                throw new Exception("Invalid identifier declaration.");
            }

            symbolTable.NewScope();
            symbolTable.EnterSymbol(node.Player.Id, node.Player);
            symbolTable.EnterSymbol(node.Opponents.Id, node.Opponents);
            node.AllPlayers.Accept(this);
            node.PlayLoopBody.Accept(this);
            node.UntilCondition.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(ProgNode node)
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
            if (!(symbolTable.RetrieveSymbol("main") is FunctionDclNode))
            {
                throw new Exception("No entry point found (Missing main func).");
            }
            node.TopDclNodes.ForEach(x => x.Accept(this));
        }

        public override void Visit(ReturnNode node)
        {
            node.ReturnValue?.Accept(this);
        }

        public override void Visit(StringValueNode node)
        {
            // Nothing
        }

        public override void Visit(StructDclNode node)
        {
            if (node.Id.IdOperations?.Count > 0)
            {
                throw new Exception("Invalid identifier declaration.");
            }

            symbolTable.NewScope();
            node.Declarations?.ForEach(x => x.Accept(this));
            node.Constructor?.Accept(this);
            symbolTable.CloseScope();
        }

        public override void Visit(UnaryExpressionNode node)
        {
            node.ExprNode.Accept(this);
        }

        public override void Visit(WhileNode node)
        {
            symbolTable.NewScope();
            node.ControlExpr.Accept(this);
            node.WhileLoopBody.Accept(this);
            symbolTable.CloseScope();
        }
    }
}
