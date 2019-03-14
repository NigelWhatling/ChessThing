namespace ChessApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Chess.Engine;

    public class ChessAppModel : ObservableObject
    {
        public List<HeaderLabel> Headers { get; set;  } = new List<HeaderLabel>(32);

        public List<Square> Squares { get; set; } = new List<Square>(64);

        public ChessGame Game { get; }

        public string Turn { get; set; }

        public string State { get; set; }

        public string Move { get; set; }

        public bool Wait { get; set; }

        public ChessAppModel()
        {
            for (int i = 0; i < 8; i++)
            {
                this.Headers.Add(new HeaderLabel { X = i + 1, Y = 0, Label = Convert.ToString((char)('a' + i)) });
                this.Headers.Add(new HeaderLabel { X = i + 1, Y = 9, Label = Convert.ToString((char)('a' + i)) });
                this.Headers.Add(new HeaderLabel { X = 0, Y = 8 - i, Label = Convert.ToString((char)('1' + i)) });
                this.Headers.Add(new HeaderLabel { X = 9, Y = 8 - i, Label = Convert.ToString((char)('1' + i)) });
            }

            for (int y = 7; y >= 0; y--)
            {
                for (int x = 0; x < 8; x++)
                {
                    this.Squares.Add(new Square
                    {
                        X = x,
                        Y = y,
                        Colour = ((x + y) % 2) == 1 ? Brushes.DarkGoldenrod : Brushes.BurlyWood,
                        Symbol = null,
                        BorderColour = Brushes.Transparent
                    });
                }
            }

            this.Game = new ChessGame();
        }

        public void Refresh()
        {
            this.Turn = $"{this.Game.Turn} ({this.Game.TurnColour})";
            this.OnPropertyChanged(() => this.Turn);

            this.State = this.Game.Board.State.ToString();
            this.OnPropertyChanged(() => this.State);

            this.Move = this.Game.Board.ExecutingMove;
            this.OnPropertyChanged(() => this.Move);

            int i = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Square square = this.Squares[i++];
                    ChessBoardSquare sq = this.Game.Board[x, y];
                    square.Symbol = Convert.ToString(sq.OccupiedBy?.Symbol);

                    square.BorderColour = sq.IsUnderAttack ? Brushes.Red : Brushes.Transparent;
                }
            }
        }

        public class HeaderLabel
        {
            public int X { get; set; }
            public int Y { get; set; }
            public string Label { get; set; }
        }

        public class Square : ObservableObject
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Brush Colour { get; set; }

            private string _Symbol;
            public string Symbol { get { return this._Symbol; } set { this.SetAndNotify(ref this._Symbol, value, () => this.Symbol); } }

            private Brush _BorderColour;
            public Brush BorderColour { get { return this._BorderColour; } set { this.SetAndNotify(ref this._BorderColour, value, () => this.BorderColour); } }
        }
    }
}
