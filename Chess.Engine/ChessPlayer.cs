using System.Collections.Generic;
using System.Linq;
using Chess.Engine.AI;

namespace Chess.Engine
{
    public class ChessPlayer
    {
        public ChessGame Game { get; }

        public ChessPlayerColour Colour { get; }

        public int Direction { get; }

        public IEnumerable<ChessPiece> Pieces => this.Game?.Board.Pieces.Where(p => p.Colour == this.Colour);

        public IEnumerable<ChessPiece> ActivePieces => this.Pieces.Where(p => p.IsOnBoard);

        public IGameplayAI AI { get; }

        public ChessPlayer(ChessGame game, ChessPlayerColour colour, int direction, IGameplayAI ai)
        {
            this.Game = game;
            this.Colour = colour;
            this.Direction = direction;
            this.AI = ai;
        }

        public IEnumerable<ChessPiece> this[ChessPieceType type] => this.ActivePieces.Where(p => p.PieceType == type);

        public ChessMove Move(ChessBoard board)
        {
            return this.AI.NextMove(board);
        }
    }
}
