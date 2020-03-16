using System;
using System.Collections.Generic;
using ScannerLib;

namespace ParserLib
{
    public class Parser
    {
        Queue<Token> tokens;

        public Parser(Queue<Token> tokenStream) 
        {
            tokens = tokenStream;
            
        }

        private void Match(TokenType tokenType)
        {
            if (tokens.Peek().Type == tokenType)
            {
                // Advance
                tokens.Dequeue();
            }
            else
            {
                // Throw exception...
            }
        }

        private void ParseProg()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.global_token, TokenType.func_token, TokenType.struct_token, TokenType.eof_token))
            {
                ParseTopDcls();
                Match(TokenType.eof_token);
            }
            else
            {
                // Expected something else.. ERROR
            }
        }

        private void ParseTopDcls()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.global_token, TokenType.func_token, TokenType.struct_token))
            {
                ParseTopDcl();
                ParseTopDcls();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.eof_token))
            {
                // Do nothing
            }
            else
            {
                // Expected this and this..
            }
        }
        
        private void ParseTopDcl()
        {

        }
        
        private void ParseGlobalDcl()
        {
            //7	GlobalDcl → global Type id = Expr ;                     	global
            if (tokens.Peek().IsInPredictSet(TokenType.global_token))
            {
                Match(TokenType.global_token);
                ParseType();
                Match(TokenType.id_token);
                Match(TokenType.assign_token);
                ParseExpr();
                Match(TokenType.semicolon_token);
            }
            else
            {
                // ERROR
            }
        }

        private void ParseStructDcl()
        {

        }
        
        private void ParseStructBlock()
        {

        }
        private void ParseConstructor()
        {

        }
        private void ParseDcls()
        {

        }
        private void ParseFunctionDcl()
        {

        }
        private void ParseFormalParams()
        {

        }
        private void ParseRemainingParams()
        {

        }
        private void ParseFormalParam()
        {

        }
        private void ParseReturnType()
        {

        }

        private void ParseBlock()
        {

        }
        private void ParseStmts()
        {

        }
        private void ParseStmt()
        {

        }
        private void ParseDcl()
        {

        }
        private void ParseAssignOrCall()
        {

        }
        private void ParseBrackets()
        {

        }
        private void ParseInit()
        {

        }
        private void ParseAssign()
        {

        }
        private void ParseIfStmt()
        {

        }
        private void ParseElifs()
        {

        }
        private void ParseElse()
        {

        }
        private void ParseWhileLoop()
        {

        }
        private void ParseReturn()
        {

        }
        private void ParseReturnValue()
        {

        }
        private void ParsePlayLoop()
        {

        }
        private void ParseType()
        {

        }
        private void ParseExpr()
        {

        }
        private void ParseString()
        {

        }
        private void ParseConcat()
        {

        }
        private void ParseBoolExpr()
        {

        }
        private void ParseOrExpr()
        {

        }
        private void ParseCompExpr1()
        {

        }
        private void ParseAndExpr()
        {

        }
        private void ParseCompExpr2()
        {

        }
        private void ParseEqualExpr()
        {

        }
        private void ParseCompExpr3()
        {

        }
        private void ParseSizeComp()
        {

        }
        private void ParseCompExpr4()
        {

        }
        private void ParseBasicBool()
        {

        }
        private void ParseArithExpr()
        {

        }

        private void ParseArithOp1()
        {

        }
        private void ParseArithExpr1()
        {

        }
        private void ParseArithOp2()
        {

        }
        private void ParseExpr2()
        {

        }
        private void ParseExpr3()
        {

        }
        private void ParseOp3()
        {

        }
        private void ParseExpr4()
        {

        }
        private void ParseIdCallOrOperations()
        {

        }
        private void ParseCall()
        {

        }
        private void ParseActualParams()
        {

        }
        private void ParseFuncValue()
        {

        }
        private void ParseIdOperations()
        {

        }
        private void ParseIdOperation()
        {

        }
    }
}
