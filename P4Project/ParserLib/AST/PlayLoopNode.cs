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
        public PlayLoopNode(IdNode Player, IdNode Opponent, IdNode AllPlayers, BlockNode PlayLoopBody, ExpressionNode UntilCondition)
        {
            this.Player = Player;
            this.Opponent = Opponent;
            this.AllPlayers = AllPlayers;
            this.PlayLoopBody = PlayLoopBody;
            this.UntilCondition = UntilCondition;
        }
    }
}
