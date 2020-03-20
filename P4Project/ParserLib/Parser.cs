using System;
using System.Collections.Generic;
using ScannerLib;
using System.Linq;
using ParserLib.AST;

namespace ParserLib
{
    public class Parser
    {
        private readonly Queue<Token> tokens;

        public Parser(Queue<Token> tokenStream) 
        {
            tokens = tokenStream;
        }

        public void StartParse()
        {
            ParseProg();
        }

        private Token Match(TokenType tokenType)
        {
            if (tokens.Peek().Type == tokenType)
            {
                // Advance
                Console.WriteLine("Advancing..");
                return tokens.Dequeue();
            }
            else
            {
                // Throw exception...
                throw new Exception($"Expected type was {tokenType}, but the token was {tokens.Peek().Type}");
            }
        }

        private ProgNode ParseProg()
        {
            ProgNode node = null;

            if (tokens.Peek().IsInPredictSet(TokenType.global_token, TokenType.func_token, TokenType.struct_token, TokenType.eof_token))
            {
                ParseTopDcls();
                Match(TokenType.eof_token);
                return node;
            }
            else
            {
                // Expected something else.. ERROR
                throw new SyntacticalException(tokens.Peek());
            }
        }

        private List<TopDclNode> ParseTopDcls()
        {
            List<TopDclNode> topNodes = new List<TopDclNode>();

            if (tokens.Peek().IsInPredictSet(TokenType.global_token, TokenType.func_token, TokenType.struct_token))
            {
                topNodes.Add(ParseTopDcl());
                topNodes.AddRange(ParseTopDcls());
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.eof_token))
            {
                // Epsilon
            }
            else
            {
                // Expected this and this..
                throw new SyntacticalException(tokens.Peek());

            }
            return topNodes;
        }
        
        private TopDclNode ParseTopDcl()
        {

            if (tokens.Peek().IsInPredictSet(TokenType.global_token))
            {
                return ParseGlobalDcl();
             
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.struct_token))
            {
                return ParseStructDcl();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.func_token))
            {
                return ParseFunctionDcl();
            }
            else
            {
                // ERROR
                throw new SyntacticalException(tokens.Peek());

            }
        }
        
        private GlobalDclNode ParseGlobalDcl()
        {
            GlobalDclNode globalNode = null;

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
               
                throw new SyntacticalException(tokens.Peek());
            }

            return globalNode;
        }

        private StructDclNode ParseStructDcl()
        {
            StructDclNode structNode = null;

            if (tokens.Peek().IsInPredictSet(TokenType.struct_token))
            {
                Match(TokenType.struct_token);
                Match(TokenType.id_token);
                ParseStructBlock();
            }
            else
            {
                // ERROR
                throw new SyntacticalException(tokens.Peek());

            }
            return structNode;
        }
        
        private Tuple<ConstructorNode, List<DeclarationNode>> ParseStructBlock()
        {
            Tuple<ConstructorNode, List<DeclarationNode>> structBlock = null;
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
                throw new SyntacticalException(tokens.Peek());

            }
            return structBlock;
            
        }
        private ConstructorNode ParseConstructor()
        {
            ConstructorNode ctorNode = null;

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
                throw new SyntacticalException(tokens.Peek());

            }
            return ctorNode;
        }
        private List<DeclarationNode> ParseDcls()
        {
            List<DeclarationNode> dclNodes = null;

            if (tokens.Peek().IsInPredictSet(TokenType.local_token))
            {
                ParseDcl();
                Match(TokenType.semicolon_token);
                ParseDcls();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token, TokenType.rcbracket_token))

            {
                // EPSILON
 
            }
            else
            {
                // ERROR
                throw new SyntacticalException(tokens.Peek());

            }
            return dclNodes;
        }
        private FunctionDclNode ParseFunctionDcl()
        {
            FunctionDclNode funcDclNode = null;

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
                throw new SyntacticalException(tokens.Peek());

            }
            return funcDclNode;
        }
        private List<FormalParamNode> ParseFormalParams()
        {
            List<FormalParamNode> formalParams = null;

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
                throw new SyntacticalException(tokens.Peek());

            }
            return formalParams;
        }
        private List<FormalParamNode> ParseRemainingParams()
        {
            List<FormalParamNode> remainingParams = null;

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
                throw new SyntacticalException(tokens.Peek());

            }
            return remainingParams;
        }
        private FormalParamNode ParseFormalParam()
        {
            FormalParamNode formalParamNode = null;

            if (tokens.Peek().IsInPredictSet(TokenType.intdcl_token, TokenType.floatdcl_token, TokenType.stringdcl_token, TokenType.booldcl_token, TokenType.id_token))
            {
                ParseType();
                Match(TokenType.id_token);
            }
            else
            {
                // ERROR
                throw new SyntacticalException(tokens.Peek());

            }
            return formalParamNode;
        }
        private string ParseReturnType()
        {
            string returnType = null;
            
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
                throw new SyntacticalException(tokens.Peek());

            }

            return returnType;
        }

        private List<StmtNode> ParseBlock()
        {
            List<StmtNode> block = null;

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
                throw new SyntacticalException(tokens.Peek());

            }
            return block;
        }
        private List<StmtNode> ParseStmts()
        {
            List<StmtNode> stmts = null;

            if(tokens.Peek().IsInPredictSet(TokenType.id_token, TokenType.local_token, TokenType.while_token,
                                            TokenType.play_token, TokenType.if_token, TokenType.return_token))
            {
                ParseStmt();
                Match(TokenType.semicolon_token);
                ParseStmts();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rcbracket_token))
            {

            }
            else
            {
                // Error
                throw new SyntacticalException(tokens.Peek());

            }
            return stmts;
        }
        private StmtNode ParseStmt()
        {
            StmtNode stmt = null;

            if (tokens.Peek().IsInPredictSet(TokenType.local_token))
            {
                ParseDcl();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                
                Token tok = Match(TokenType.id_token);
                stmt = ParseAssignOrCall(tok);
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
                throw new SyntacticalException(tokens.Peek());

            }
            return stmt;
        }
        private DeclarationNode ParseDcl()
        {
            DeclarationNode dcl = null;

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
                throw new SyntacticalException(tokens.Peek());

            }
            return dcl;
        }

        private StmtNode ParseAssignOrCall(Token tok)
        {
            StmtNode assignOrCall = null;

            if (tokens.Peek().IsInPredictSet(TokenType.assign_token, TokenType.lsbracket_token, TokenType.dot_token))
            {
                Tuple<List<IdOperationNode>, ExpressionNode> assign = ParseAssign();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lparen_token))
            {
                ParseCall();
            }
            else
            {
                // Error
                throw new SyntacticalException(tokens.Peek());

            }
            return assignOrCall;
        }
        private bool ParseBrackets()
        {
            bool isArray = false;

            if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token))
            {
                Match(TokenType.lsbracket_token);
                Match(TokenType.rsbracket_token);
                isArray = true;
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
            }
            else
            {
                // Error
                throw new SyntacticalException(tokens.Peek());

            }
            return isArray;
        }
        private ExpressionNode ParseInit()
        {
            ExpressionNode init = null;

            if (tokens.Peek().IsInPredictSet(TokenType.assign_token))
            {
                Match(TokenType.assign_token);
                ParseExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
            }
            else
            {
                // Error
                throw new SyntacticalException(tokens.Peek());

            }
            return init;

        }
        private Tuple<List<IdOperationNode>, ExpressionNode> ParseAssign()
        {
            Tuple<List<IdOperationNode>, ExpressionNode> assign = null;

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
                throw new SyntacticalException(tokens.Peek());

            }
            return assign;

        }
        private IfNode ParseIfStmt()
        {
            IfNode ifStmt = null;

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
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return ifStmt;

        }
        private List<ElifNode> ParseElifs()
        {
            List<ElifNode> elifs = null;

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
            }
            else
            {
                // ERROR
                throw new SyntacticalException(tokens.Peek());
            }
            return elifs;

        }
        private ElseNode ParseElse()
        {
            ElseNode elseNode = null;

            // Else -> else Block | EPSILON
            if (tokens.Peek().IsInPredictSet(TokenType.else_token))
            {
                Match(TokenType.else_token);
                ParseBlock();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return elseNode;

        }
        private WhileNode ParseWhileLoop()
        {
            WhileNode whileNode = null;

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
                throw new SyntacticalException(tokens.Peek());
            }
            return whileNode;
        }

        private ReturnNode ParseReturn()
        {
            ReturnNode returnNode = null;

            // Return -> return ReturnValue
            if (tokens.Peek().IsInPredictSet(TokenType.return_token))
            {
                Match(TokenType.return_token);
                ParseReturnValue();
            }
            else
            {
                // ERROR
                throw new SyntacticalException(tokens.Peek());
            }
            return returnNode;
        }

        private ExpressionNode ParseReturnValue()
        {
            ExpressionNode returnValue = null;

            // ReturnValue -> Expr | EPSILON
            if (tokens.Peek().IsInPredictSet(TokenType.stringval_token, TokenType.not_token, TokenType.boolval_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return returnValue;
        }
        
        private PlayLoopNode ParsePlayLoop()
        {
            PlayLoopNode playLoop = null;

            // PlayLoop -> play ( id vs id in id IdCallOrOperations ) Block until ( BoolExpr )
            if (tokens.Peek().IsInPredictSet(TokenType.play_token))
            {
                Match(TokenType.play_token);
                Match(TokenType.lparen_token);
                Match(TokenType.id_token);
                Match(TokenType.vs_token);
                Match(TokenType.id_token);
                Match(TokenType.in_token);
                Token tok = Match(TokenType.id_token);
                ParseIdCallOrOperations(tok);
                Match(TokenType.rparen_token);
                ParseBlock();
                Match(TokenType.until_token);
                Match(TokenType.lparen_token);
                ParseBoolExpr();
                Match(TokenType.rparen_token);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return playLoop;

        }
        private string ParseType()
        {
            string parseType = null;

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
                throw new SyntacticalException(tokens.Peek());
            }
            return parseType;
        }

        private ExpressionNode ParseExpr()
        {
            ExpressionNode expr = null;

            if(tokens.Peek().IsInPredictSet(TokenType.stringval_token, TokenType.not_token, TokenType.boolval_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                expr = ParseString();
                ExpressionNode exprNode = ParseConcat();
                if(exprNode != null)
                {
                    //BinaryExpressionNode binaryNode = new BinaryExpressionNode 
                    // return binaryNode; 
                }                   
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return expr;
        }

        private ExpressionNode ParseString()
        {
            ExpressionNode expr = null;

            if (tokens.Peek().IsInPredictSet(TokenType.stringval_token))
            {
                Match(TokenType.stringval_token);
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.not_token, TokenType.boolval_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseBoolExpr();
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return expr;
        }

        private ExpressionNode ParseConcat()
        {
            ExpressionNode expr = null;

            if (tokens.Peek().IsInPredictSet(TokenType.colon_token))
            {
                Match(TokenType.colon_token);
                ParseString();
                ParseConcat();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.comma_token, TokenType.rparen_token, TokenType.semicolon_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return expr;
        }

        private ExpressionNode ParseBoolExpr()
        {
            ExpressionNode boolExpr = null;

            if(tokens.Peek().IsInPredictSet(TokenType.not_token, TokenType.boolval_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                // Samme fremgangsmåde som parseExpr :))
                ParseCompExpr1();
                ParseOrExpr();
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return boolExpr;
        }

        private ExpressionNode ParseOrExpr()
        {
            ExpressionNode orExpr = null;

            if (tokens.Peek().IsInPredictSet(TokenType.or_token))
            {
                Match(TokenType.or_token);
                ParseBoolExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return orExpr;
        }

        private ExpressionNode ParseCompExpr1()
        {
            ExpressionNode compExpr1 = null;

            if(tokens.Peek().IsInPredictSet(TokenType.not_token, TokenType.boolval_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                // Samme fremgangsmåde som parseExpr :))
                ParseCompExpr2();
                ParseAndExpr();
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return compExpr1;
        }

        private ExpressionNode ParseAndExpr()
        {
            ExpressionNode andExpr = null;

            if (tokens.Peek().IsInPredictSet(TokenType.and_token))
            {
                Match(TokenType.and_token);
                ParseCompExpr1();
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return andExpr;
        }

        private ExpressionNode ParseCompExpr2()
        {
            ExpressionNode compExpr2 = null;

            if(tokens.Peek().IsInPredictSet(TokenType.not_token, TokenType.boolval_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                // Samme fremgangsmåde som parseExpr :))
                ParseCompExpr3();
                ParseEqualExpr();
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return compExpr2;
        }

        private Tuple<ExpressionNode, TokenType> ParseEqualExpr()
        {
            Tuple<ExpressionNode, TokenType> equalExpr = null;

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
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return equalExpr;
        }

        private ExpressionNode ParseCompExpr3()
        {
            ExpressionNode compExpr3 = null;

            // Samme fremgangsmåde som parseExpr :))
            if(tokens.Peek().IsInPredictSet(TokenType.not_token, 
                                            TokenType.boolval_token, 
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
                throw new SyntacticalException(tokens.Peek());
            }
            return compExpr3;
        }
        private Tuple<ExpressionNode, TokenType> ParseSizeComp()
        {
            Tuple < ExpressionNode, TokenType > sizeComp = null;

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
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return sizeComp;
        }
        private Tuple<ExpressionNode, TokenType> ParseCompExpr4()
        {
            Tuple<ExpressionNode, TokenType> compExpr4 = null;

            if (tokens.Peek().IsInPredictSet(TokenType.not_token))
            {
                Match(TokenType.not_token);
                ParseBasicBool();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.boolval_token,
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
                throw new SyntacticalException(tokens.Peek());
            }
            return compExpr4;
        }
        private ExpressionNode ParseBasicBool()
        {
            ExpressionNode basicBool = null;

            if (tokens.Peek().IsInPredictSet(TokenType.minus_token,
                                             TokenType.lparen_token,
                                             TokenType.inum_token,
                                             TokenType.fnum_token,
                                             TokenType.id_token))
            {
                ParseArithExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.boolval_token))
            {
                Match(TokenType.boolval_token);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return basicBool;
        }
        private ExpressionNode ParseArithExpr()
        {
            ExpressionNode arithExpr = null;

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
                throw new SyntacticalException(tokens.Peek());
            }
            return arithExpr;
        }
        private Tuple<ExpressionNode, TokenType> ParseArithOp1()
        {
            Tuple < ExpressionNode, TokenType > arithOp1 = null;

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
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return arithOp1;
        }
        private ExpressionNode ParseArithExpr1()
        {
            ExpressionNode arithExpr1 = null;

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
                throw new SyntacticalException(tokens.Peek());
            }
            return arithExpr1;
        }
        private Tuple<ExpressionNode, TokenType> ParseArithOp2()
        {
            Tuple<ExpressionNode, TokenType> arithOp2 = null;

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
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return arithOp2;
        }
        private Tuple<ExpressionNode, TokenType> ParseArithExpr2()
        {
            Tuple<ExpressionNode, TokenType> arithExpr2 = null;

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
                throw new SyntacticalException(tokens.Peek());
            }
            return arithExpr2;
        }
        private ExpressionNode ParseArithExpr3()
        {
            ExpressionNode arithExpr3 = null;

            if (tokens.Peek().IsInPredictSet(TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ParseArithExpr4();
                ParseArithOp3();
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return arithExpr3;
        }
        private Tuple<ExpressionNode, TokenType> ParseArithOp3()
        {
            Tuple<ExpressionNode, TokenType> arithOp3 = null;

            if (tokens.Peek().IsInPredictSet(TokenType.power_token))
            {
                Match(TokenType.power_token);
                ParseArithExpr3();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.multiply_token, TokenType.divide_token, TokenType.modulo_token, TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token)){
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return arithOp3;
        }

        private ExpressionNode ParseArithExpr4()
        {
            ExpressionNode arithValue = null;

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
                Token tok = Match(TokenType.id_token);
                ParseIdCallOrOperations(tok);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return arithValue;
        }

        private ExpressionNode ParseIdCallOrOperations(Token tok)
        {
            ExpressionNode callOrOperations = null;

            if (tokens.Peek().IsInPredictSet(TokenType.lparen_token))
            {
                ParseCall();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token, TokenType.dot_token, TokenType.assign_token, TokenType.rparen_token, TokenType.power_token, TokenType.multiply_token, TokenType.divide_token, TokenType.modulo_token, TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token)){
                ParseIdOperations();
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return callOrOperations;
        }


        private List<ExpressionNode> ParseCall()
        {
            List<ExpressionNode> call = null;

            if (tokens.Peek().IsInPredictSet(TokenType.lparen_token)){
                Match(TokenType.lparen_token);
                ParseActualParams();
                Match(TokenType.rparen_token);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return call;
        }

        private List<ExpressionNode> ParseActualParams()
        {
            List<ExpressionNode> actualParams = null;

            if (tokens.Peek().IsInPredictSet(TokenType.stringval_token, TokenType.not_token, TokenType.boolval_token, TokenType.minus_token, TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token)){
                ParseExpr();
                ParseFuncValue();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return actualParams;
        }
        private List<ExpressionNode> ParseFuncValue()
        {
            List<ExpressionNode> funcValue = null;

            if (tokens.Peek().IsInPredictSet(TokenType.comma_token))
            {
                Match(TokenType.comma_token);
                ParseExpr();
                ParseFuncValue();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return funcValue;
        }
        private List<IdOperationNode> ParseIdOperations()
        {
            List<IdOperationNode> idOperation = null;

            if(tokens.Peek().IsInPredictSet(TokenType.lsbracket_token, TokenType.dot_token))
            {
                ParseIdOperation();
                ParseIdOperations();
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.assign_token, TokenType.rparen_token, TokenType.power_token, TokenType.multiply_token, TokenType.divide_token, TokenType.modulo_token, TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return idOperation;
        }
        private IdOperationNode ParseIdOperation()
        {
            IdOperationNode idOperation = null;

            if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token))
            {
                Match(TokenType.lsbracket_token);
                ExpressionNode exprNode = ParseArithExpr();
                Match(TokenType.rsbracket_token);
                idOperation = new ArrayAccessNode(exprNode);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.dot_token)){
                Match(TokenType.dot_token);
                Match(TokenType.id_token);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return idOperation;
        }
    }
}
