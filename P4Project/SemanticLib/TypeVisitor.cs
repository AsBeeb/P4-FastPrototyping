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
            throw new NotImplementedException();
        }

        public override void Visit(AssignmentNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BinaryExpressionNode node)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void Visit(WhileNode node)
        {
            throw new NotImplementedException();
        }
    }
}
