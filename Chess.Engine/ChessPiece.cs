namespace Chess.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ChessPiece : IEquatable<ChessPiece>
    {
        private int _number;
        private int _lastMoveTurn = -1;

        public ChessBoard Board { get; }

        public ChessPlayerColour Colour { get; }

        public ChessPlayerColour OpponentColour => (ChessPlayerColour)(((int)this.Colour + 1) % 2);

        public ChessPieceType PieceType { get; private set; }

        public ChessLocation Location { get; private set; }

        public int Direction { get; }

        public bool HasMoved { get; private set; }

        public bool JustMoved => this._lastMoveTurn == this.Board.Turn - 1;

        public bool IsOnBoard => this.Location.IsOnBoard;

        public bool IsUnderAttack => this.Colour != this.Board.TurnColour && this.Board[this.Location].IsUnderAttack;

        public string Id => string.Format("{0}{1}", this.Code, this._number > 0 ? this._number.ToString() : string.Empty);

        public char Code
        {
            get
            {
                if (this.PieceType == ChessPieceType.Knight)
                {
                    return 'N';
                }
                else
                {
                    return this.PieceType.ToString()[0];
                }
            }
        }

        public char Symbol
        {
            get
            {
                switch (this.PieceType)
                {
                    case ChessPieceType.King:
                        return this.Colour == ChessPlayerColour.White ? '♔' : '♚';

                    case ChessPieceType.Queen:
                        return this.Colour == ChessPlayerColour.White ? '♕' : '♛';

                    case ChessPieceType.Bishop:
                        return this.Colour == ChessPlayerColour.White ? '♗' : '♝';

                    case ChessPieceType.Knight:
                        return this.Colour == ChessPlayerColour.White ? '♘' : '♞';

                    case ChessPieceType.Rook:
                        return this.Colour == ChessPlayerColour.White ? '♖' : '♜';

                    case ChessPieceType.Pawn:
                        return this.Colour == ChessPlayerColour.White ? '♙' : '♟';
                }

                return '?';
            }
        }

        public ChessPiece(ChessBoard board, ChessPlayerColour colour, ChessPieceType type, ChessLocation location, int direction)
        {
            this.Board = board;
            this.Colour = colour;
            this.PieceType = type;
            this.Location = location;
            this.Direction = direction;
        }

        public ChessPiece(ChessBoard board, ChessPlayerColour colour, ChessPieceType type, int number, ChessLocation location, int direction)
            : this(board, colour, type, location, direction)
        {
            this._number = number;
        }

        public ChessPiece Clone(ChessBoard board)
        {
            return new ChessPiece(board, this.Colour, this.PieceType, this._number, this.Location, this.Direction);
        }

        public bool Move(ChessLocation location)
        {
            // TODO: Validation
            this.Location = location;
            this.HasMoved = true;
            this._lastMoveTurn = this.Board.Turn;
            return true;
        }

        public void Capture()
        {
            this.Location = ChessLocation.OffBoard;
        }

        public void Promote(ChessPieceType type)
        {
            this.PieceType = type;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ChessPiece);
        }

        public bool Equals(ChessPiece other)
        {
            return other != null &&
                   this._number == other._number &&
                   EqualityComparer<ChessBoard>.Default.Equals(this.Board, other.Board) &&
                   this.Colour == other.Colour &&
                   this.PieceType == other.PieceType;
        }

        public override int GetHashCode()
        {
            var hashCode = -1345084272;
            hashCode = hashCode * -1521134295 + this._number.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ChessBoard>.Default.GetHashCode(this.Board);
            hashCode = hashCode * -1521134295 + this.Colour.GetHashCode();
            hashCode = hashCode * -1521134295 + this.PieceType.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ChessPiece piece1, ChessPiece piece2)
        {
            return EqualityComparer<ChessPiece>.Default.Equals(piece1, piece2);
        }

        public static bool operator !=(ChessPiece piece1, ChessPiece piece2)
        {
            return !(piece1 == piece2);
        }
    }
}
