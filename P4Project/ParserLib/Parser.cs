using System;
using System.Collections.Generic;
using ScannerLib;
using System.Linq;
using ParserLib.AST;
using System.Globalization;

namespace ParserLib
{
    public class Parser
    {
        private readonly Queue<Token> tokens;

        public Parser(Queue<Token> tokenStream) 
        {
            tokens = tokenStream;
        }

        public ProgNode StartParse()
        {
            return ParseProg();
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
                throw new Exception($"Line: {tokens.Peek().Line}. Expected type was {tokenType}, but the token was {tokens.Peek().Type}");
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
                globalNode = new GlobalDclNode(id, initVal, type, false);
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
                returnType = "void";
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

            if (tokens.Peek().IsInPredictSet(TokenType.lsbracket_token, TokenType.dot_token, TokenType.assign_token))
            {
                List<IdOperationNode> idsOpsList = ParseIdOperations(); 
                Match(TokenType.assign_token);
                ExpressionNode exprNode = ParseExpr();

                assign = new Tuple<List<IdOperationNode>, ExpressionNode>(idsOpsList, exprNode);
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

            if (tokens.Peek().IsInPredictSet(TokenType.if_token))
            {
                Match(TokenType.if_token);
                Match(TokenType.lparen_token);
                ExpressionNode controlExpr = ParseBoolExpr();
                Match(TokenType.rparen_token);
                BlockNode ifBody = ParseBlock();
                List<ElifNode> elifNodes = ParseElifs();
                ElseNode elseNode = ParseElse();

                ifStmt = new IfNode(controlExpr, ifBody, elifNodes, elseNode);
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

            if (tokens.Peek().IsInPredictSet(TokenType.elif_token))
            {
                Match(TokenType.elif_token);
                Match(TokenType.lparen_token);
                ExpressionNode exprNode = ParseBoolExpr();
                Match(TokenType.rparen_token);
                BlockNode blockBody = ParseBlock();

                ElifNode elifNode = new ElifNode(exprNode, blockBody);
                elifs.Add(elifNode);

                elifs.AddRange(ParseElifs());
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.else_token, TokenType.semicolon_token))
            {
                // Advance
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
                BlockNode blockBody = ParseBlock();

                elseNode = new ElseNode(blockBody);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
                // Advance
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

            if (tokens.Peek().IsInPredictSet(TokenType.while_token))
            {
                Match(TokenType.while_token);
                Match(TokenType.lparen_token);
                ExpressionNode controlExpr = ParseBoolExpr();
                Match(TokenType.rparen_token);
                BlockNode whileLoopBody = ParseBlock();

                whileNode = new WhileNode(controlExpr, whileLoopBody);
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

            if (tokens.Peek().IsInPredictSet(TokenType.return_token))
            {
                Match(TokenType.return_token);
                ExpressionNode returnValue = ParseReturnValue();

                returnNode = new ReturnNode(returnValue);
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
                returnValue = ParseExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.semicolon_token))
            {
                // Advance
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

            if (tokens.Peek().IsInPredictSet(TokenType.play_token))
            {
                Match(TokenType.play_token);
                Match(TokenType.lparen_token);
                Token playerHandle = Match(TokenType.id_token);
                IdNode playerNode = new IdNode(playerHandle.Value, null);
                Match(TokenType.vs_token);
                Token otherPlayersHandle = Match(TokenType.id_token);
                IdNode otherNode = new IdNode(otherPlayersHandle.Value, null);
                Match(TokenType.in_token);
                Token allPlayers = Match(TokenType.id_token);
                ExpressionNode allPlayerNode = ParseIdCallOrOperations(allPlayers);
                Match(TokenType.rparen_token);
                BlockNode loopBodyNode = ParseBlock();
                Match(TokenType.until_token);
                Match(TokenType.lparen_token);
                ExpressionNode boolExprNode = ParseBoolExpr();
                Match(TokenType.rparen_token);

                playLoop = new PlayLoopNode(playerNode, otherNode, allPlayerNode, loopBodyNode, boolExprNode); 
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

            if (tokens.Peek().IsInPredictSet(TokenType.intdcl_token))
            {
                Match(TokenType.intdcl_token);
                parseType = "int";
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.floatdcl_token))
            {
                Match(TokenType.floatdcl_token);
                parseType = "float";
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.stringdcl_token))
            {
                Match(TokenType.stringdcl_token);
                parseType = "string";
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.booldcl_token))
            {
                Match(TokenType.booldcl_token);
                parseType = "bool";
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.id_token))
            {
                parseType = Match(TokenType.id_token).Value;
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
                    BinaryExpressionNode binaryNode = new BinaryExpressionNode(expr, exprNode, BinaryOperator.STRING_CONCAT);
                    return binaryNode; 
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
                expr = ParseBoolExpr();
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
                expr = ParseString();
                ExpressionNode concatNode = ParseConcat();
                if(concatNode != null)
                {
                    BinaryExpressionNode binExprNode = new BinaryExpressionNode(expr, concatNode, BinaryOperator.STRING_CONCAT);
                    return binExprNode;
                }
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.comma_token, TokenType.rparen_token, TokenType.semicolon_token))
            {
                // Epsilon
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
                boolExpr = ParseCompExpr1();
                ExpressionNode orExpr = ParseOrExpr();
                if(orExpr != null)
                {
                    BinaryExpressionNode binExprNode = new BinaryExpressionNode(boolExpr, orExpr, BinaryOperator.OR);
                    return binExprNode;
                }
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
                orExpr = ParseBoolExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                // Epsilon
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
                compExpr1 = ParseCompExpr2();
                ExpressionNode andExpr = ParseAndExpr();
                if(andExpr != null)
                {
                    BinaryExpressionNode binExpressionNode = new BinaryExpressionNode(compExpr1, andExpr, BinaryOperator.AND);
                    return binExpressionNode;
                }
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
                andExpr = ParseCompExpr1();
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                // Epsilon
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
                compExpr2 = ParseCompExpr3();
                Tuple<ExpressionNode, TokenType> equalExprNode = ParseEqualExpr();
                if(equalExprNode != null)
                {
                    BinaryExpressionNode binExprNode;
                    if (equalExprNode.Item2 == TokenType.equal_token)
                    {
                        binExprNode = new BinaryExpressionNode(compExpr2, equalExprNode.Item1, BinaryOperator.EQUALS);
                        return binExprNode;
                    }
                    else if(equalExprNode.Item2 == TokenType.notequal_token)
                    {
                        binExprNode = new BinaryExpressionNode(compExpr2, equalExprNode.Item1, BinaryOperator.NOT_EQUALS);
                        return binExprNode;
                    }
                }
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
                ExpressionNode exprNode = ParseCompExpr2();
                equalExpr = new Tuple<ExpressionNode, TokenType>(exprNode, TokenType.equal_token);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.notequal_token))
            {
                Match(TokenType.notequal_token);
                ExpressionNode exprNode2 = ParseCompExpr2();
                equalExpr = new Tuple<ExpressionNode, TokenType>(exprNode2, TokenType.notequal_token);
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.and_token, TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                // Epsilon
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

            if(tokens.Peek().IsInPredictSet(TokenType.not_token, 
                                            TokenType.boolval_token, 
                                            TokenType.minus_token,
                                            TokenType.lparen_token,
                                            TokenType.inum_token,
                                            TokenType.fnum_token,
                                            TokenType.id_token))
            {
                Tuple<ExpressionNode, TokenType> compExpr4 = ParseCompExpr4();
                compExpr3 = compExpr4.Item1;

                if (compExpr4.Item2 != TokenType.default_token)
                {
                    UnaryOperator Operator = GetUnaryOperator(compExpr4.Item2); // Not operator
                    compExpr3 = new UnaryExpressionNode(compExpr3, Operator);
                }

                Tuple<ExpressionNode, TokenType> sizeComp = ParseSizeComp();
                if (sizeComp != null)
                {
                    ExpressionNode sizeCompExpr = sizeComp.Item1;

                    if (sizeComp.Item2 != TokenType.default_token)
                    {
                        BinaryOperator Operator = GetBinaryOperator(sizeComp.Item2); // Not operator
                        compExpr3 = new BinaryExpressionNode(compExpr3, sizeCompExpr, Operator);
                    }
                }
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
                TokenType tokenType = Match(TokenType.greaterorequal_token).Type;
                Tuple<ExpressionNode, TokenType> compExpr4 = ParseCompExpr4();
                ExpressionNode compExpr4Expr = compExpr4.Item1;

                if (compExpr4.Item2 != TokenType.default_token)
                {
                    UnaryOperator Operator = GetUnaryOperator(compExpr4.Item2); // Not operator
                    compExpr4Expr = new UnaryExpressionNode(compExpr4Expr, Operator);
                }
                sizeComp = new Tuple<ExpressionNode, TokenType>(compExpr4Expr, tokenType);
            } 
            else if (tokens.Peek().IsInPredictSet(TokenType.lessorequal_token))
            {
                TokenType tokenType = Match(TokenType.lessorequal_token).Type;
                Tuple<ExpressionNode, TokenType> compExpr4 = ParseCompExpr4();
                ExpressionNode compExpr4Expr = compExpr4.Item1;

                if (compExpr4.Item2 != TokenType.default_token)
                {
                    UnaryOperator Operator = GetUnaryOperator(compExpr4.Item2); // Not operator
                    compExpr4Expr = new UnaryExpressionNode(compExpr4Expr, Operator);
                }
                sizeComp = new Tuple<ExpressionNode, TokenType>(compExpr4Expr, tokenType);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lessthan_token))
            {
                TokenType tokenType = Match(TokenType.lessthan_token).Type;
                Tuple<ExpressionNode, TokenType> compExpr4 = ParseCompExpr4();
                ExpressionNode compExpr4Expr = compExpr4.Item1;

                if (compExpr4.Item2 != TokenType.default_token)
                {
                    UnaryOperator Operator = GetUnaryOperator(compExpr4.Item2); // Not operator
                    compExpr4Expr = new UnaryExpressionNode(compExpr4Expr, Operator);
                }
                sizeComp = new Tuple<ExpressionNode, TokenType>(compExpr4Expr, tokenType);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.greaterthan_token))
            {
                TokenType tokenType = Match(TokenType.greaterthan_token).Type;
                Tuple<ExpressionNode, TokenType> compExpr4 = ParseCompExpr4();
                ExpressionNode compExpr4Expr = compExpr4.Item1;

                if (compExpr4.Item2 != TokenType.default_token)
                {
                    UnaryOperator Operator = GetUnaryOperator(compExpr4.Item2); // Not operator
                    compExpr4Expr = new UnaryExpressionNode(compExpr4Expr, Operator);
                }
                sizeComp = new Tuple<ExpressionNode, TokenType>(compExpr4Expr, tokenType);
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
                TokenType tokenType = Match(TokenType.not_token).Type;
                ExpressionNode BasicBool = ParseBasicBool();
                compExpr4 = new Tuple<ExpressionNode, TokenType>(BasicBool, tokenType);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.boolval_token,
                                                  TokenType.minus_token,
                                                  TokenType.lparen_token,
                                                  TokenType.inum_token,
                                                  TokenType.fnum_token,
                                                  TokenType.id_token))
            {
                ExpressionNode BasicBool = ParseBasicBool();
                TokenType tokenType = TokenType.default_token;
                compExpr4 = new Tuple<ExpressionNode, TokenType>(BasicBool, tokenType);
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
                basicBool = ParseArithExpr();
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.boolval_token))
            {
                bool value = Boolean.Parse(Match(TokenType.boolval_token).Value);
                basicBool = new BoolValueNode(value);
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
                ExpressionNode ArithExpr1 = ParseArithExpr1();
                arithExpr = ArithExpr1;
                Tuple<ExpressionNode, TokenType> arithOp1 = ParseArithOp1();
                if (arithOp1 != null)
                {
                    BinaryOperator Operator = GetBinaryOperator(arithOp1.Item2);
                    arithExpr = new BinaryExpressionNode(arithExpr, arithOp1.Item1, Operator);
                }
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
                TokenType tokenType = Match(TokenType.plus_token).Type;
                ExpressionNode ArithExpr = ParseArithExpr();
                arithOp1 = new Tuple<ExpressionNode, TokenType>(ArithExpr, tokenType);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.minus_token))
            {
                TokenType tokenType = Match(TokenType.minus_token).Type;
                ExpressionNode ArithExpr = ParseArithExpr();
                arithOp1 = new Tuple<ExpressionNode, TokenType>(ArithExpr, tokenType);
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
                UnaryExpressionNode unary = null;
                BinaryExpressionNode binary = null;
                // find ud af om det er en unary operator
                Tuple <ExpressionNode, TokenType> arithexpr2 = ParseArithExpr2();
                arithExpr1 = arithexpr2.Item1;
                
                if (arithexpr2.Item2 != TokenType.default_token)
                {
                    UnaryOperator Operator = GetUnaryOperator(arithexpr2.Item2);
                    unary = new UnaryExpressionNode(arithexpr2.Item1, Operator);
                    arithExpr1 = unary;
                }
                
                //find ud af om dette skal ende med at være en binaryexpression
                Tuple<ExpressionNode, TokenType> arithop2 = ParseArithOp2();
                if (arithop2 != null)
                {
                    BinaryOperator Operator = GetBinaryOperator(arithop2.Item2);
                    binary = new BinaryExpressionNode(arithExpr1, arithop2.Item1, Operator);
                    arithExpr1 = binary;
                }
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

                Token operatorToken = Match(TokenType.multiply_token);
                TokenType operatorType = operatorToken.Type;
                ExpressionNode ArithExpr1 = ParseArithExpr1();

                arithOp2 = new Tuple<ExpressionNode, TokenType>(ArithExpr1, operatorType);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.divide_token)){
                Token operatorToken = Match(TokenType.divide_token);
                TokenType operatorType = operatorToken.Type;
                ExpressionNode ArithExpr1 = ParseArithExpr1();

                arithOp2 = new Tuple<ExpressionNode, TokenType>(ArithExpr1, operatorType);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.modulo_token)) {
                Token operatorToken = Match(TokenType.modulo_token);
                TokenType operatorType = operatorToken.Type;
                ExpressionNode ArithExpr1 = ParseArithExpr1();

                arithOp2 = new Tuple<ExpressionNode, TokenType>(ArithExpr1, operatorType);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.rparen_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                // EPSILON
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
                Token Operator = Match(TokenType.minus_token);
                TokenType OperatorType = Operator.Type;

                ExpressionNode aritheexpr3 = ParseArithExpr3();

                arithExpr2 = new Tuple<ExpressionNode, TokenType>(aritheexpr3, OperatorType);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.lparen_token, TokenType.inum_token, TokenType.fnum_token, TokenType.id_token))
            {
                ExpressionNode aritheexpr3 = ParseArithExpr3();
                arithExpr2 = new Tuple<ExpressionNode, TokenType>(aritheexpr3, TokenType.default_token); 
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
                ExpressionNode arithExpr4 = ParseArithExpr4();

                arithExpr3 = arithExpr4;
                Tuple<ExpressionNode, TokenType> ArithOp3Tuple = ParseArithOp3();
                if (ArithOp3Tuple != null)
                {
                    BinaryOperator Operator = GetBinaryOperator(ArithOp3Tuple.Item2);
                    arithExpr3 = new BinaryExpressionNode(arithExpr3, ArithOp3Tuple.Item1, Operator);
                }
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
                Token token = Match(TokenType.power_token);
                TokenType tokenType = token.Type;
                ExpressionNode ArithExpr3 = ParseArithExpr3();

                arithOp3 = new Tuple<ExpressionNode, TokenType>(ArithExpr3, tokenType);
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
                CultureInfo cultureInfo = new CultureInfo("en-US");
                Token floatToken = Match(TokenType.fnum_token);
                float floatValue = float.Parse(floatToken.Value, cultureInfo);
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
                // EPSILON
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
                List<ExpressionNode> parseFuncValue = ParseFuncValue();
                if (parseFuncValue != null)
                    funcValues.AddRange(parseFuncValue);
            }
            else if (tokens.Peek().IsInPredictSet(TokenType.rparen_token))
            {
                // EPSILON
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
                List<IdOperationNode> parseIdOperations = ParseIdOperations();
                if (parseIdOperations != null)
                    idOperations.AddRange(parseIdOperations);
            }
            else if(tokens.Peek().IsInPredictSet(TokenType.assign_token, TokenType.rparen_token, TokenType.power_token, TokenType.multiply_token, TokenType.divide_token, TokenType.modulo_token, TokenType.plus_token, TokenType.minus_token, TokenType.rsbracket_token, TokenType.greaterorequal_token, TokenType.lessorequal_token, TokenType.lessthan_token, TokenType.greaterthan_token, TokenType.equal_token, TokenType.notequal_token, TokenType.and_token, TokenType.or_token, TokenType.colon_token, TokenType.comma_token, TokenType.semicolon_token))
            {
                // EPSILON
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

        private UnaryOperator GetUnaryOperator(TokenType tokenType)
        {
            UnaryOperator Operator = UnaryOperator.DEFAULT;
            switch (tokenType){
                case TokenType.not_token:
                    Operator = UnaryOperator.NOT;
                    break;
                case TokenType.minus_token:
                    Operator = UnaryOperator.UNARY_MINUS;
                    break;
            }
            return Operator;
        }

        private BinaryOperator GetBinaryOperator(TokenType tokenType)
        {
            BinaryOperator Operator = BinaryOperator.DEFAULT;

            switch (tokenType)
            {
                //Logic operators
                case TokenType.greaterorequal_token:
                    Operator = BinaryOperator.GREATER_OR_EQUALS;
                    break;
                case TokenType.greaterthan_token:
                    Operator = BinaryOperator.GREATER_THAN;
                    break;
                case TokenType.lessorequal_token:
                    Operator = BinaryOperator.LESS_OR_EQUALS;
                    break;
                case TokenType.lessthan_token:
                    Operator = BinaryOperator.LESS_THAN;
                    break;
                case TokenType.and_token:
                    Operator = BinaryOperator.AND;
                    break;
                case TokenType.or_token:
                    Operator = BinaryOperator.OR;
                    break;
                case TokenType.notequal_token:
                    Operator = BinaryOperator.NOT_EQUALS;
                    break;
                case TokenType.equal_token:
                    Operator = BinaryOperator.EQUALS;
                    break;
                    //aritmetic operators
                case TokenType.minus_token:
                    Operator = BinaryOperator.MINUS;
                    break;
                case TokenType.plus_token:
                    Operator = BinaryOperator.PLUS;
                    break;
                case TokenType.multiply_token:
                    Operator = BinaryOperator.MULTIPLY;
                    break;
                case TokenType.divide_token:
                    Operator = BinaryOperator.DIVIDE;
                    break;
                case TokenType.modulo_token:
                    Operator = BinaryOperator.MODULO;
                    break;
                case TokenType.power_token:
                    Operator = BinaryOperator.POWER;
                    break;
                    //String operator
                case TokenType.colon_token:
                    Operator = BinaryOperator.STRING_CONCAT;
                    break;
            }

            return Operator;
        }
    }
}
