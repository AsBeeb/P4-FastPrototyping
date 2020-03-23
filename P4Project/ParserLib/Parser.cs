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
                List<TopDclNode> topDclNodes = ParseTopDcls();
                Match(TokenType.eof_token);
                node = new ProgNode(topDclNodes);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return node;
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
                // Epsilon.
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return topNodes;
        }
        
        private TopDclNode ParseTopDcl()
        {
            TopDclNode topDclNode = null;
            
            if (tokens.Peek().IsInPredictSet(TokenType.global_token))
            {
                topDclNode = ParseGlobalDcl();            
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.struct_token))
            {
                topDclNode = ParseStructDcl();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.func_token))
            {
                topDclNode = ParseFunctionDcl();
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }

            return topDclNode;
        }
        
        private GlobalDclNode ParseGlobalDcl()
        {
            GlobalDclNode globalNode = null;

            if (tokens.Peek().IsInPredictSet(TokenType.global_token))
            {
                Match(TokenType.global_token);
                string type = ParseType();
                IdNode id = new IdNode(Match(TokenType.id_token).Value, null);
                Match(TokenType.assign_token);
                ExpressionNode initVal = ParseExpr();
                Match(TokenType.semicolon_token);
                globalNode = new GlobalDclNode(id, initVal, type);
            }
            else
            {             
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
                IdNode id = new IdNode(Match(TokenType.id_token).Value, null);
                var (constructor, declarations) = ParseStructBlock();
                structNode = new StructDclNode(id, declarations, constructor);
            }
            else
            {
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
                List<DeclarationNode> dcls = ParseDcls();
                ConstructorNode constructor = ParseConstructor();
                Match(TokenType.rcbracket_token);
                structBlock = new Tuple<ConstructorNode, List<DeclarationNode>>(constructor, dcls);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return structBlock;
            
        }
        private ConstructorNode ParseConstructor()
        {
            ConstructorNode ctorNode = null;

            if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                IdNode id = new IdNode(Match(TokenType.id_token).Value, null);
                Match(TokenType.lparen_token);
                List<FormalParamNode> formalParams = ParseFormalParams();
                Match(TokenType.rparen_token);
                BlockNode block = ParseBlock();
                ctorNode = new ConstructorNode(id, formalParams, block);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rcbracket_token))
            {
                // EPSILON
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return ctorNode;
        }
        private List<DeclarationNode> ParseDcls()
        {
            List<DeclarationNode> dclNodes = new List<DeclarationNode>();

            if (tokens.Peek().IsInPredictSet(TokenType.local_token))
            {
                dclNodes.Add(ParseDcl());
                Match(TokenType.semicolon_token);
                dclNodes.AddRange(ParseDcls());
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token, TokenType.rcbracket_token))
            {
                // EPSILON
            }
            else
            {
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
                string returnType = ParseReturnType();
                IdNode id = new IdNode(Match(TokenType.id_token).Value, null);
                Match(TokenType.lparen_token);
                List<FormalParamNode> formalParams = ParseFormalParams();
                Match(TokenType.rparen_token);
                BlockNode block = ParseBlock();
                funcDclNode = new FunctionDclNode(id, returnType, formalParams, block);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return funcDclNode;
        }
        private List<FormalParamNode> ParseFormalParams()
        {
            List<FormalParamNode> formalParams = new List<FormalParamNode>();

            if (tokens.Peek().IsInPredictSet(TokenType.intdcl_token, TokenType.floatdcl_token, TokenType.stringdcl_token, TokenType.booldcl_token, TokenType.id_token))
            {
                formalParams.Add(ParseFormalParam());
                formalParams.AddRange(ParseRemainingParams());
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
                // EPSILON                
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return formalParams;
        }
        private List<FormalParamNode> ParseRemainingParams()
        {
            List<FormalParamNode> remainingParams = new List<FormalParamNode>();

            if (tokens.Peek().IsInPredictSet(TokenType.comma_token))
            {
                Match(TokenType.comma_token);
                remainingParams.Add(ParseFormalParam());
                remainingParams.AddRange(ParseRemainingParams());
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
                // EPSILON
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return remainingParams;
        }
        private FormalParamNode ParseFormalParam()
        {
            FormalParamNode formalParamNode = null;

            if (tokens.Peek().IsInPredictSet(TokenType.intdcl_token, TokenType.floatdcl_token, TokenType.stringdcl_token, TokenType.booldcl_token, TokenType.id_token))
            {
                string type = ParseType();
                IdNode id = new IdNode (Match(TokenType.id_token).Value, null);
                formalParamNode = new FormalParamNode(id, type);
            }
            else
            {
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
                returnType = ParseType();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.void_token))
            {
                Match(TokenType.void_token);
                returnType = "Void";
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }

            return returnType;
        }

        private BlockNode ParseBlock()
        {
            BlockNode block = null;
            var stmts = new List<StmtNode>();

            if (tokens.Peek().IsInPredictSet(TokenType.lcbracket_token)){
                Match(TokenType.lcbracket_token);
                stmts.Add(ParseStmt());
                Match(TokenType.semicolon_token);
                stmts.AddRange(ParseStmts());
                Match(TokenType.rcbracket_token);
                block = new BlockNode(stmts);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return block;
        }
        private List<StmtNode> ParseStmts()
        {
            List<StmtNode> stmts = new List<StmtNode>();

            if(tokens.Peek().IsInPredictSet(TokenType.id_token, TokenType.local_token, TokenType.while_token,
                                            TokenType.play_token, TokenType.if_token, TokenType.return_token))
            {
                stmts.Add(ParseStmt());
                Match(TokenType.semicolon_token);
                stmts.AddRange(ParseStmts());
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rcbracket_token))
            {

            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return stmts;
        }
        private StmtNode ParseStmt()
        {
            StmtNode stmt = null;

            if (tokens.Peek().IsInPredictSet(TokenType.local_token))
            {
                stmt = ParseDcl();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                
                Token tok = Match(TokenType.id_token);
                stmt = ParseAssignOrCall(tok);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.if_token))
            {
                stmt = ParseIfStmt();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.while_token))
            {
                stmt = ParseWhileLoop();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.return_token))
            {
                stmt = ParseReturn();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.play_token))
            {
                stmt = ParsePlayLoop();
            }
            else
            {
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
                string type = ParseType();
                bool isArray = ParseBrackets();
                IdNode id = new IdNode(Match(TokenType.id_token).Value, null);
                ExpressionNode init = ParseInit();
                dcl = new DeclarationNode(id, type, init, isArray);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return dcl;
        }

        private StmtNode ParseAssignOrCall(Token idToken)
        {
            StmtNode assignOrCall = null;

            if (tokens.Peek().IsInPredictSet(TokenType.assign_token, TokenType.lsbracket_token, TokenType.dot_token))
            {
                Tuple<List<IdOperationNode>, ExpressionNode> assign = ParseAssign();
                assignOrCall = new AssignmentNode(new IdNode(idToken.Value, assign.Item1), assign.Item2);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lparen_token))
            {
                List<ExpressionNode> actualParams = ParseCall();
                assignOrCall = new FuncCallStmtNode(new IdNode(idToken.Value, null), actualParams);
            }
            else
            {
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
                // EPSILON
            }
            else
            {
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
                init = ParseExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
                // EPSILON.
            }
            else
            {
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
                Token tok = Match(TokenType.stringval_token);
                expr = new StringValueNode(tok.Value);
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
                arithValue = ParseExpr();
                Match(TokenType.rparen_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.inum_token))
            {
                Token intToken = Match(TokenType.inum_token);
                int intValue = int.Parse(intToken.Value);
                arithValue = new IntValueNode(intValue);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.fnum_token))
            {
                Token floatToken = Match(TokenType.fnum_token);
                float floatValue = float.Parse(floatToken.Value);
                arithValue = new FloatValueNode(floatValue);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                Token tok = Match(TokenType.id_token);
                arithValue = ParseIdCallOrOperations(tok);
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
                IdNode id = new IdNode(tok.Value, null);
                callOrOperations = new FuncCallExpressionNode(id, ParseCall());
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token, TokenType.dot_token, TokenType.assign_token, TokenType.rparen_token, TokenType.power_token, TokenType.multiply_token, TokenType.divide_token, TokenType.modulo_token, TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token)){
                callOrOperations = new IdExpressionNode(tok.Value, ParseIdOperations());
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
                call = ParseActualParams();
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
                actualParams = new List<ExpressionNode>();
                actualParams.Add(ParseExpr());
                actualParams.AddRange(ParseFuncValue());
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
            List<ExpressionNode> funcValues = null;

            if (tokens.Peek().IsInPredictSet(TokenType.comma_token))
            {
                funcValues = new List<ExpressionNode>();
                Match(TokenType.comma_token);
                funcValues.Add(ParseExpr());
                funcValues.AddRange(ParseFuncValue());
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return funcValues;
        }
        private List<IdOperationNode> ParseIdOperations()
        {
            List<IdOperationNode> idOperations = null;

            if(tokens.Peek().IsInPredictSet(TokenType.lsbracket_token, TokenType.dot_token))
            {
                idOperations = new List<IdOperationNode>();
                idOperations.Add(ParseIdOperation());
                idOperations.AddRange(ParseIdOperations());
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.assign_token, TokenType.rparen_token, TokenType.power_token, TokenType.multiply_token, TokenType.divide_token, TokenType.modulo_token, TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return idOperations;
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
                IdNode id = new IdNode(Match(TokenType.id_token).Value, null);
                idOperation = new FieldAccessNode(id);
            }
            else
            {
                throw new SyntacticalException(tokens.Peek());
            }
            return idOperation;
        }
    }
}
