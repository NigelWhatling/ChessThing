using System;
using System.Linq;

namespace Chess.Engine
{
    public class ChessBoardState
    {
        private readonly ChessBoardSquare[,] _boardState = new ChessBoardSquare[8, 8];

        public ChessBoardState()
        {
            this.Reset();
        }

        public ChessBoardSquare this[ChessLocation location] => this[location.X, location.Y];

        public ChessBoardSquare this[int x, int y]
        {
            get
            {
                if (x < 0 || x > 7 || y < 0 || y > 7)
                {
                    return new ChessBoardSquare(ChessLocation.OffBoard);
                }

                return this._boardState[x, y];
            }
        }

        public void Reset()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    this._boardState[x, y] = new ChessBoardSquare(new ChessLocation(x, y));
                }
            }
        }

        //public static ChessBoardState CreateBoardState(ChessGame game)
        //{
        //    ChessBoardState state = new ChessBoardState();

        //    foreach (ChessPiece piece in game.Board.ActivePieces)
        //    {
        //        piece.IsUnderAttack = false;
        //        state[piece.Location.x, piece.Location.y].OccupiedBy = piece;
        //    }

        //    foreach (ChessPiece piece in game.CurrentOpponent.ActivePieces)
        //    {
        //        foreach (ChessMove move in piece.GetMoves().Where(m => m.IsAttackingMove))
        //        {
        //            state[move.to.x, move.to.y].IsUnderAttack = true;
        //            if (state[move.to.x, move.to.y].IsOccupied && state[move.to.x, move.to.y].OccupiedBy.Player == game.CurrentPlayer)
        //            {
        //                state[move.to.x, move.to.y].OccupiedBy.IsUnderAttack = true;
        //            }
        //        }
        //    }

        //    return state;
        //}

        //public static ChessBoardState CreateBoardStateFromSnapshot(ChessGame game, string snapshot)
        //{
        //    ChessBoardState state = new ChessBoardState();

        //    byte[] data = Convert.FromBase64String(snapshot);
            

        //    this.Pieces.Add(new ChessPiece(this, ChessPieceType.King) { Location = new ChessLocation(4, baseY) });
        //    this.Pieces.Add(new ChessPiece(this, ChessPieceType.Queen) { Location = new ChessLocation(3, baseY) });
        //    this.Pieces.Add(new ChessPiece(this, ChessPieceType.Bishop, 1) { Location = new ChessLocation(2, baseY) });
        //    this.Pieces.Add(new ChessPiece(this, ChessPieceType.Bishop, 2) { Location = new ChessLocation(5, baseY) });
        //    this.Pieces.Add(new ChessPiece(this, ChessPieceType.Knight, 1) { Location = new ChessLocation(1, baseY) });
        //    this.Pieces.Add(new ChessPiece(this, ChessPieceType.Knight, 2) { Location = new ChessLocation(6, baseY) });
        //    this.Pieces.Add(new ChessPiece(this, ChessPieceType.Rook, 1) { Location = new ChessLocation(0, baseY) });
        //    this.Pieces.Add(new ChessPiece(this, ChessPieceType.Rook, 2) { Location = new ChessLocation(7, baseY) });

        //    for (int i = 0; i <= 7; i++)
        //    {
        //        this.Pieces.Add(new ChessPiece(this, ChessPieceType.Pawn, i + 1) { Location = new ChessLocation(i, baseY + this.Direction) });
        //    }

        //    foreach (ChessPiece piece in this.ActivePieces)
        //    {
        //        piece.IsUnderAttack = false;
        //        this._boardState[piece.Location.x, piece.Location.y].OccupiedBy = piece;
        //    }

        //    foreach (ChessPiece piece in this.Game.CurrentOpponent.ActivePieces)
        //    {
        //        foreach (ChessMove move in piece.GetMoves().Where(m => m.IsAttackingMove))
        //        {
        //            this._boardState[move.to.x, move.to.y].IsUnderAttack = true;
        //            if (this._boardState[move.to.x, move.to.y].IsOccupied && this._boardState[move.to.x, move.to.y].OccupiedBy.Player == this.Game.CurrentPlayer)
        //            {
        //                this._boardState[move.to.x, move.to.y].OccupiedBy.IsUnderAttack = true;
        //            }
        //        }
        //    }

        //    return state;
        //}
    }
}
