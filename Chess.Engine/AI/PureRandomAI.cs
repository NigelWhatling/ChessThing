namespace Chess.Engine.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PureRandomAI : IGameplayAI
    {
        private Random _rng = new Random();

        public PureRandomAI()
        {
        }

        public ChessMove NextMove(ChessBoard board)
        {
            List<ChessMove> moves = board.GetAllCurrentMoves().ToList();
            if (moves.Count == 0)
            {
                return new ChessMove(ChessLocation.OffBoard, ChessLocation.OffBoard);
            }

            ChessMove move = moves[_rng.Next(moves.Count)];

            if (board[move.from].OccupiedBy.PieceType == ChessPieceType.Pawn && (move.to.Y == 0 || move.to.Y == 7))
            {
                move.promoteTo = (ChessPieceType)_rng.Next(1, 5);
            }

            return move;
        }
    }
}
