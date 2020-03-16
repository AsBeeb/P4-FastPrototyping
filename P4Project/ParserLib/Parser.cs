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
            if(tokens.Peek().IsInPredictSet(TokenType.intdcl_token, TokenType.floatdcl_token, TokenType.stringdcl_token,
                                            TokenType.booldcl_token, TokenType.id_token))
            {
                ParseType();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.void_token))
            {
                Match(TokenType.void_token);
            }
            else
            {
                // Error
            }
        }

        private void ParseBlock()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.lcbracket_token)){
                Match(TokenType.lcbracket_token);
                ParseStmt();
                Match(TokenType.semicolon_token);
                ParseStmts();
                Match(TokenType.rcbracket_token);
            }
            else
            {
                // Error
            }
        }
        private void ParseStmts()
        {
            if(tokens.Peek().IsInPredictSet(TokenType.id_token, TokenType.local_token, TokenType.while_token,
                                            TokenType.play_token, TokenType.if_token, TokenType.return_token))
            {
                ParseStmt();
                Match(TokenType.semicolon_token);
                ParseStmts();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rcbracket_token))
            {
                return;
            }
            else
            {
                // Error
            }
        }
        private void ParseStmt()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.local_token))
            {
                ParseDcl();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                Match(TokenType.id_token);
                ParseAssignOrCall();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.if_token))
            {
                ParseIfStmt();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.while_token))
            {
                ParseWhileLoop();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.return_token))
            {
                ParseReturn();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.play_token))
            {
                ParsePlayLoop();
            }
            else
            {
                // Error
            }
        }
        private void ParseDcl()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.local_token))
            {
                Match(TokenType.local_token);
                ParseType();
                ParseBrackets();
                Match(TokenType.id_token);
                ParseInit();
            }
            else
            {
                // Error
            }
        }
        private void ParseAssignOrCall()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.assign_token, TokenType.lsbracket_token, TokenType.dot_token))
            {
                ParseAssign();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lparen_token))
            {
                ParseCall();
            }
            else
            {
                // Error
            }
        }
        private void ParseBrackets()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token))
            {
                Match(TokenType.lsbracket_token);
                Match(TokenType.rsbracket_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                return;
            }
            else
            {
                // Error
            }
        }
        private void ParseInit()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.assign_token))
            {
                Match(TokenType.assign_token);
                ParseExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // Error
            }

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
