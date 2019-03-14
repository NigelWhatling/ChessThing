namespace Chess.Engine
{
    public class ChessBoardSquare
    {
        public ChessLocation Location { get; }

        public int X => this.Location.X;
        public int Y => this.Location.Y;

        public ChessPiece OccupiedBy { get; set; }

        public ChessPlayerColour OccupiedByColour => this.IsOccupied ? this.OccupiedBy.Colour : ChessPlayerColour.Vacant;

        public bool IsOccupied => this.OccupiedBy != null;

        public bool IsUnderAttack { get; set; }

        public bool CanAttack { get; set; }

        public bool IsOnBoard => this.Location != ChessLocation.OffBoard;

        public ChessBoardSquare(int x, int y)
            : this(new ChessLocation(x, y))
        { }

        public ChessBoardSquare(ChessLocation location)
        {
            this.Location = location;
            this.OccupiedBy = null;
            this.IsUnderAttack = false;
        }

        public void Reset()
        {
            this.OccupiedBy = null;
            this.IsUnderAttack = false;
        }
    }
}
