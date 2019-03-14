using System;
using System.Collections.Generic;

namespace Chess.Engine
{
    public struct ChessMove : IEquatable<ChessMove>
    { 
        public ChessLocation from;
        public ChessLocation to;

        public ChessLocation? targetLocation;
        public ChessLocation? castleFrom;
        public ChessLocation? castleTo;
        public ChessPieceType? promoteTo;

        public bool IsAttackingMove;

        public bool IsValid => this.from.IsOnBoard && this.to.IsOnBoard;

        public ChessMove(ChessLocation from, ChessLocation to)
        {
            this.from = from;
            this.to = to;
            this.IsAttackingMove = true;
            this.targetLocation = null;
            this.castleFrom = null;
            this.castleTo = null;
            this.promoteTo = null;
        }

        public ChessMove(ChessLocation from, ChessLocation to, bool isAttackingMove = true)
        {
            this.from = from;
            this.to = to;
            this.IsAttackingMove = isAttackingMove;
            this.targetLocation = null;
            this.castleFrom = null;
            this.castleTo = null;
            this.promoteTo = null;
        }

        public override bool Equals(object obj) => obj is ChessMove && this.Equals((ChessMove)obj);
        public bool Equals(ChessMove other) => EqualityComparer<ChessLocation>.Default.Equals(this.from, other.from) && EqualityComparer<ChessLocation>.Default.Equals(this.to, other.to);

        public override int GetHashCode()
        {
            var hashCode = -1951484959;
            hashCode = hashCode * -1521134295 + EqualityComparer<ChessLocation>.Default.GetHashCode(this.from);
            hashCode = hashCode * -1521134295 + EqualityComparer<ChessLocation>.Default.GetHashCode(this.to);
            return hashCode;
        }

        public override string ToString() => $"{this.from}-{this.to}";

        public static bool operator ==(ChessMove move1, ChessMove move2) => move1.Equals(move2);
        public static bool operator !=(ChessMove move1, ChessMove move2) => !(move1 == move2);
    }
}
