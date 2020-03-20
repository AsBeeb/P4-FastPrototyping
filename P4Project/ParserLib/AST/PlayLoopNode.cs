using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class PlayLoopNode : StmtNode
    {
        public IdNode Player;
        public IdNode Opponent;
        public IdNode AllPlayers;
        public BlockNode PlayLoopBody;
        public ExpressionNode UntilCondition;
    }
}
