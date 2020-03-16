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
            if (tokens.Peek().IsInPredictSet(TokenType.global_token))
            {
                ParseGlobalDcl();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.struct_token))
            {
                ParseStructDcl();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.func_token))
            {
                ParseFunctionDcl();
            }
            else
            {
                // ERROR
            }
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
            if (tokens.Peek().IsInPredictSet(TokenType.struct_token))
            {
                Match(TokenType.struct_token);
                Match(TokenType.id_token);
                ParseStructBlock();
            }
            else
            {
                // ERROR
            }
        }
        
        private void ParseStructBlock()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.lcbracket_token))
            {
                Match(TokenType.lcbracket_token);
                ParseDcls();
                ParseConstructor();
                Match(TokenType.rcbracket_token);
            }
            else
            {
                // ERROR
            }
        }
        private void ParseConstructor()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                Match(TokenType.id_token);
                Match(TokenType.lparen_token);
                ParseFormalParams();
                Match(TokenType.rparen_token);
                ParseBlock();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rcbracket_token))
            {
                // EPSILON
            }
            else
            {
                // ERROR
            }
        }
        private void ParseDcls()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.local_token))
            {
                ParseDcl();
                Match(TokenType.semicolon_token);
                ParseDcls();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                // EPSILON
            }
            else
            {
                // ERROR
            }
        }
        private void ParseFunctionDcl()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.func_token))
            {
                Match(TokenType.func_token);
                ParseReturnType();
                Match(TokenType.id_token);
                Match(TokenType.lparen_token);
                ParseFormalParams();
                Match(TokenType.rparen_token);
                ParseBlock();
            }
            else
            {
                // ERROR
            }
        }
        private void ParseFormalParams()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.intdcl_token, TokenType.floatdcl_token, TokenType.stringdcl_token, TokenType.booldcl_token, TokenType.id_token))
            {
                ParseFormalParam();
                ParseRemainingParams();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
                // EPSILON
            }
            else
            {
                // ERROR
            }
        }
        private void ParseRemainingParams()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.comma_token))
            {
                Match(TokenType.comma_token);
                ParseFormalParam();
                ParseRemainingParams();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
                // EPSILON
            }
            else
            {
                // ERROR
            }
        }
        private void ParseFormalParam()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.intdcl_token, TokenType.floatdcl_token, TokenType.stringdcl_token, TokenType.booldcl_token, TokenType.id_token))
            {
                ParseType();
                Match(TokenType.id_token);
            }
            else
            {
                // ERROR
            }
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
            // Assign -> IdOperations = Expr 
            if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token, TokenType.dot_token, TokenType.assign_token))
            {
                ParseIdOperations();
                Match(TokenType.assign_token);
                ParseExpr();
            }
            else
            {
                // ERROR
            }

        }
        private void ParseIfStmt()
        {
            // IfStmt-> if (BoolExpr) Block Elifs Else
            if (tokens.Peek().IsInPredictSet(TokenType.if_token))
            {
                Match(TokenType.if_token);
                Match(TokenType.lparen_token);
                ParseBoolExpr();
                Match(TokenType.rparen_token);
                ParseBlock();
                ParseElifs();
                ParseElse();
            }

        }
        private void ParseElifs()
        {
            // Elifs->elif(BoolExpr) Block Elifs | EPSILON
            if (tokens.Peek().IsInPredictSet(TokenType.elif_token))
            {
                Match(TokenType.elif_token);
                Match(TokenType.lparen_token);
                ParseBoolExpr();
                Match(TokenType.rparen_token);
                ParseBlock();
                ParseElifs();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.else_token, TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // ERROR
            }

        }
        private void ParseElse()
        {
            // Else -> else Block | EPSILON
            if (tokens.Peek().IsInPredictSet(TokenType.else_token))
            {
                Match(TokenType.else_token);
                ParseBlock();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // ERROR 
            }

        }
        private void ParseWhileLoop()
        {
            // WhileLoop -> while ( BoolExpr ) Block
            if (tokens.Peek().IsInPredictSet(TokenType.while_token))
            {
                Match(TokenType.while_token);
                Match(TokenType.lparen_token);
                ParseBoolExpr();
                Match(TokenType.rparen_token);
                ParseBlock();
            }
            else
            {
                // ERROR
            }
        }

        private void ParseReturn()
        {
            // Return -> return ReturnValue
            if (tokens.Peek().IsInPredictSet(TokenType.return_token))
            {
                Match(TokenType.return_token);
                ParseReturnValue();
            }
            else
            {
                // ERROR
            }
        }

        private void ParseReturnValue()
        {
            // ReturnValue -> Expr | EPSILON
            if (tokens.Peek().IsInPredictSet(TokenType.stringval_token, TokenType.not_token, TokenType.booldcl_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // ERROR
            }
        }
        
        private void ParsePlayLoop()
        {
            // PlayLoop -> play ( id vs id in id IdCallOrOperations ) Block until ( BoolExpr )
            if (tokens.Peek().IsInPredictSet(TokenType.play_token))
            {
                Match(TokenType.play_token);
                Match(TokenType.lparen_token);
                Match(TokenType.id_token);
                Match(TokenType.vs_token);
                Match(TokenType.id_token);
                Match(TokenType.in_token);
                Match(TokenType.id_token);
                ParseIdCallOrOperations();
                Match(TokenType.rparen_token);
                ParseBlock();
                Match(TokenType.until_token);
                Match(TokenType.lparen_token);
                ParseBoolExpr();
                Match(TokenType.rparen_token);
            }
            else
            {
                // ERROR
            }

        }
        private void ParseType()
        {
            // Type -> intdcl | floatdcl | stringdcl | booldcl | id
            if (tokens.Peek().IsInPredictSet(TokenType.intdcl_token))
            {
                Match(TokenType.intdcl_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.floatdcl_token))
            {
                Match(TokenType.floatdcl_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.stringdcl_token))
            {
                Match(TokenType.stringdcl_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.booldcl_token))
            {
                Match(TokenType.booldcl_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                Match(TokenType.id_token);
            }
            else
            {
                // ERROR
            }
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
            if(tokens.Peek().IsInPredictSet(TokenType.not_token, 
                                            TokenType.booldcl_token, 
                                            TokenType.minus_token,
                                            TokenType.lparen_token,
                                            TokenType.inum_token,
                                            TokenType.fnum_token,
                                            TokenType.id_token))
            {
                ParseCompExpr4();
                ParseSizeComp();
            }
            else
            {
                // Error
            }
        }
        private void ParseSizeComp()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.greaterorequal_token))
            {
                Match(TokenType.greaterorequal_token);
                ParseCompExpr4();
            } 
            else if (tokens.Peek().IsInPredictSet(TokenType.lessorequal_token))
            {
                Match(TokenType.lessorequal_token);
                ParseCompExpr4();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lessthan_token))
            {
                Match(TokenType.lessthan_token);
                ParseCompExpr4();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.greaterthan_token))
            {
                Match(TokenType.greaterthan_token);
                ParseCompExpr4();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.equal_token,
                                                  TokenType.notequal_token, 
                                                  TokenType.and_token, 
                                                  TokenType.or_token, 
                                                  TokenType.rparen_token, 
                                                  TokenType.colon_token, 
                                                  TokenType.comma_token, 
                                                  TokenType.semicolon_token))
            {
                // Epsilon - Do nothing
                return;
            }
            else
            {
                // Error
            }
        }
        private void ParseCompExpr4()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.not_token))
            {
                Match(TokenType.not_token);
                ParseBasicBool();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.booldcl_token,
                                                  TokenType.minus_token,
                                                  TokenType.lparen_token,
                                                  TokenType.inum_token,
                                                  TokenType.fnum_token,
                                                  TokenType.id_token))
            {
                ParseBasicBool();
            }
            else
            {
                // Error
            }
        }
        private void ParseBasicBool()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.minus_token,
                                             TokenType.lparen_token,
                                             TokenType.inum_token,
                                             TokenType.fnum_token,
                                             TokenType.id_token))
            {
                ParseArithExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.booldcl_token))
            {
                Match(TokenType.booldcl_token);
            }
            else
            {
                // Error
            }
        }
        private void ParseArithExpr()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.minus_token,
                                             TokenType.lparen_token,
                                             TokenType.inum_token,
                                             TokenType.fnum_token,
                                             TokenType.id_token))
            {
                ParseArithExpr1();
                ParseArithOp1();
            }
            else
            {
                // Error
            }
        }
        private void ParseArithOp1()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.plus_token))
            {
                Match(TokenType.plus_token);
                ParseArithExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.minus_token))
            {
                Match(TokenType.minus_token);
                ParseArithExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rsbracket_token,
                                                  TokenType.greaterorequal_token,
                                                  TokenType.lessorequal_token,
                                                  TokenType.lessthan_token,
                                                  TokenType.greaterthan_token,
                                                  TokenType.equal_token,
                                                  TokenType.notequal_token,
                                                  TokenType.and_token,
                                                  TokenType.or_token,
                                                  TokenType.rparen_token,
                                                  TokenType.colon_token,
                                                  TokenType.comma_token,
                                                  TokenType.semicolon_token))
            {
                // Epsilon - Do nothing
                return;
            }
            else
            {
                // Error
            }
        }
        private void ParseArithExpr1()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.minus_token,
                                             TokenType.lparen_token,
                                             TokenType.inum_token,
                                             TokenType.fnum_token,
                                             TokenType.id_token))
            {
                ParseArithExpr2();
                ParseArithOp2();
            }
            else
            {
                // Error
            }
        }
        private void ParseArithOp2()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.multiply_token)) {
                Match(TokenType.multiply_token);
                ParseArithExpr1();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.divide_token)){
                Match(TokenType.divide_token);
                ParseArithExpr1();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.modulo_token)) {
                Match(TokenType.modulo_token);
                ParseArithExpr1();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                return;
            }
            else
            {
                // Error
            }
        }
        private void ParseArithExpr2()
        {
            if (tokens.Peek().IsInPredictSet(TokenType.minus_token))
            {
                Match(TokenType.minus_token);
                ParseArithExpr3();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseArithExpr3();
            }
            else
            {
                // Error
            }
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
