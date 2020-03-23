using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLib.AST
{
    public class PlayLoopNode : StmtNode
    {
        public IdNode Player;
        public IdNode Opponent;
        public ExpressionNode AllPlayers;
        public BlockNode PlayLoopBody;
        public ExpressionNode UntilCondition;

        public PlayLoopNode(IdNode player, IdNode opponent, ExpressionNode allPlayers, BlockNode playLoopBody, ExpressionNode untilCondition)
        {
            this.Player = player;
            this.Opponent = opponent;
            this.AllPlayers = allPlayers;
            this.PlayLoopBody = playLoopBody;
            this.UntilCondition = untilCondition;
        }
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }
}
