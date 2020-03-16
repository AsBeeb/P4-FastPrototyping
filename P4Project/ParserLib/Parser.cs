﻿using System;
using System.Collections.Generic;
using ScannerLib;

namespace ParserLib
{
    public class Parser
    {
        private readonly Queue<Token> tokens;

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
            if(tokens.Peek().IsInPredictSet(TokenType.stringval_token, TokenType.not_token, TokenType.booldcl_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseString();
                ParseConcat();
            }
            else
            {
                // Error
            }
        }

        private void ParseString()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.stringval_token))
            {
                Match(TokenType.stringval_token);
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.not_token, TokenType.booldcl_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseBoolExpr();
            }
            else
            {
                // Error
            }
        }

        private void ParseConcat()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.colon_token))
            {
                ParseString();
                ParseConcat();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.comma_token, TokenType.rparen_token, TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // Error
            }
        }

        private void ParseBoolExpr()
        {
            if(tokens.Peek().IsInPredictSet(TokenType.not_token, TokenType.booldcl_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseCompExpr1();
                ParseOrExpr();
            }
            else
            {
                // Error
            }
        }

        private void ParseOrExpr()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.or_token))
            {
                Match(TokenType.or_token);
                ParseBoolExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // Error
            }
        }

        private void ParseCompExpr1()
        {
            if(tokens.Peek().IsInPredictSet(TokenType.not_token, TokenType.booldcl_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseCompExpr2();
                ParseAndExpr();
            }
            else
            {
                // Error
            }
        }

        private void ParseAndExpr()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.and_token))
            {
                Match(TokenType.and_token);
                ParseCompExpr1();
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // Error
            }
        }

        private void ParseCompExpr2()
        {
            if(tokens.Peek().IsInPredictSet(TokenType.not_token, TokenType.booldcl_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseCompExpr3();
                ParseEqualExpr();
            }
            else
            {
                // Error
            }
        }

        private void ParseEqualExpr()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.equal_token))
            {
                Match(TokenType.equal_token);
                ParseCompExpr2();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.notequal_token))
            {
                Match(TokenType.notequal_token);
                ParseCompExpr2();
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.and_token, TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // Error
            }
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
        private void ParseArithExpr3()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseArithExpr4();
                ParseArithOp3();
            }
            else
            {
                // ERROR
            }
        }
        private void ParseArithOp3()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.power_token))
            {
                Match(TokenType.power_token);
                ParseArithExpr3();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.multiply_token, TokenType.divide_token, TokenType.modulo_token, TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token)){
                return;
            }
            else
            {
                //Error
            }
        }

        private void ParseArithExpr4()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.lparen_token)){
                Match(TokenType.lparen_token);
                ParseExpr();
                Match(TokenType.rparen_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.inum_token))
            {
                Match(TokenType.inum_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.fnum_token))
            {
                Match(TokenType.fnum_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                Match(TokenType.id_token);
                ParseIdCallOrOperations();
            }
            else
            {
                //Error
            }
        }

        private void ParseIdCallOrOperations()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.lparen_token))
            {
                ParseCall();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token)){
                ParseIdOperations();
                Match(TokenType.dot_token);
            }
            else
            {
                //Error
            }
        }

        private void ParseCall()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.lparen_token)){
                Match(TokenType.lparen_token);
                ParseActualParams();
                Match(TokenType.rparen_token);
            }
            else
            {
                //Error
            }
        }

        private void ParseActualParams()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.stringval_token, TokenType.not_token, TokenType.boolval_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token)){
                ParseExpr();
                ParseFuncValue();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
                return;
            }
            else
            {
                //Error
            }
        }
        private void ParseFuncValue()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.comma_token))
            {
                Match(TokenType.comma_token);
                ParseExpr();
                ParseFuncValue();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
                return;
            }
            else
            {
                //Error
            }
        }
        private void ParseIdOperations()
        {
            if(tokens.Peek().IsInPredictSet(TokenType.lsbracket_token, TokenType.dot_token))
            {
                ParseIdOperation();
                ParseIdOperations();
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.multiply_token, TokenType.divide_token, TokenType.modulo_token, TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                //Error
            }
        }
        private void ParseIdOperation()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token))
            {
                Match(TokenType.lsbracket_token);
                ParseArithExpr();
                Match(TokenType.rsbracket_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.dot_token)){
                Match(TokenType.and_token);
                Match(TokenType.id_token);
            }
            else
            {
                //Error
            }
        }
    }
}
