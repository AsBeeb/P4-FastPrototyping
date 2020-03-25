using ParserLib.AST;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        internal override void Visit(AssignmentNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(BinaryExpressionNode node)
        {
            node.LeftExpr.Accept(this);
            //Print rigtig operator evt. med switch/dic
            node.RightExpr.Accept(this);
        }

        internal override void Visit(BlockNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(BoolValueNode node)
        {
            PrettyPrint(node.BoolValue.ToString());
        }

        internal override void Visit(ConstructorNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(DeclarationNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(ElifNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(ElseNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(ExpressionNode node)
        {
            throw new NotImplementedException();
        }

        internal override void Visit(FieldAccessNode node)
        {
            node.Id.Accept(this);
        }

        internal override void Visit(FloatValueNode node)
        {
            PrettyPrint(node.FloatValue.ToString());
        }

        internal override void Visit(FormalParamNode node)
        {
            node.Id.Accept(this);
            PrettyPrint(node.Type);
        }

        internal override void Visit(FuncCallExpressionNode node)
        {
            node.Id.Accept(this);
            node.ActualParameters.ForEach(x => x.Accept(this));
        }

        internal override void Visit(FuncCallStmtNode node)
        {
            node.Id.Accept(this);
            node.ActualParameters.ForEach(x => x.Accept(this));
        }

        internal override void Visit(FunctionDclNode node)
        {
            PrettyPrint("func " + node.ReturnType);
            node.Id.Accept(this);
            PrettyPrint(" (");
            node.FormalParamNodes.ForEach(x => x.Accept(this));
            PrettyPrint(") \n");
            node.FuncBody.Accept(this);
        }

        internal override void Visit(GlobalDclNode node)
        {
            PrettyPrint("global " + node.Type + " "); // global int 
            node.Id.Accept(this);
            PrettyPrint(" = ");
            node.InitialValue.Accept(this);
        }

        internal override void Visit(IdExpressionNode node)
        {
            PrettyPrint(node.Id);
            node.IdOperations.ForEach(x => {
                PrettyPrint(".");
                x.Accept(this); 
            });
        }

        internal override void Visit(IdNode node)
        {
            PrettyPrint(node.Id);
            node.IdOperations.ForEach(x => {
                PrettyPrint(".");
                x.Accept(this);
            });
        }

        internal override void Visit(IfNode node)
        {
            PrettyPrint("if (" + node.ControlExpression + ") \n");
            node.IfBody.Accept(this);
            node.ElifNodes.ForEach(x => x.Accept(this));
            node.ElseNode.Accept(this);
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
