using System;

namespace Chess.Engine
{
    public struct ChessLocation : IEquatable<ChessLocation>
    {
        public static ChessLocation OffBoard = new ChessLocation(-1, -1);

        public int X { get; set; }
        public int Y { get; set; }

        public bool IsOnBoard => this.X >= 0 && this.X <= 7 && this.Y >= 0 && this.Y <= 7;

        public bool IsLightSquare => (((this.X * 8) + this.Y) % 2) > 0;

        public ChessLocation(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public ChessLocation(string code)
        {
            this.X = code.ToUpper()[0] - 'A';
            this.Y = code[1] - '1';
        }

        public static ChessLocation FromCoord(int x, int y)
        {
            return new ChessLocation(x, y);
        }

        public override bool Equals(object obj) => obj is ChessLocation && this.Equals((ChessLocation)obj);
        public bool Equals(ChessLocation other) => this.X == other.X && this.Y == other.Y;

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + this.X.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Y.GetHashCode();
            return hashCode;
        }

        public override string ToString() => string.Concat((char)(this.X + 'A'), (char)(this.Y + '1'));

        public static bool operator ==(ChessLocation location1, ChessLocation location2) => location1.Equals(location2);
        public static bool operator !=(ChessLocation location1, ChessLocation location2) => !(location1 == location2);
    }
}
