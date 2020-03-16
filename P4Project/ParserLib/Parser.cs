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
                // Do nothing
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
                // Do nothing
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
                // Do nothing
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
