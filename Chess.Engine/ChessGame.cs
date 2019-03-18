namespace Chess.Engine
{
    using System;
    using Chess.Engine.AI;
    using Chess.Engine.IO;

    public class ChessGame
    {
        public event EventHandler BeforeMove;

        public event EventHandler AfterMove;

        public ChessBoard Board { get; }

        public ChessPlayer[] Players { get; }

        public ChessPlayer CurrentPlayer => this.Players[(this.Turn + 1) % 2];

        public ChessPlayer CurrentOpponent => this.Players[this.Turn % 2];

        public TimeSpan LastTurnDuration { get; private set; }

        public int Turn => this.Board.Turn;

        public ChessPlayerColour TurnColour => this.CurrentPlayer.Colour;

        public bool CurrentPlayerInCheck { get; private set; }

        private ZobristKeyTable _keyTable = new ZobristKeyTable(14 * 64);

        public ChessGame()
        {
            this.Players = new ChessPlayer[2]
            {
                new ChessPlayer(this, ChessPlayerColour.White, 1, new PureRandomAI()),
                new ChessPlayer(this, ChessPlayerColour.Black, -1, new PureRandomAI())
            };

            this.Board = new ChessBoard(this);
            this.Reset();

            this._keyTable.Save("keytable");
        }

        public void Reset() => this.Board.Reset();

        public void AutoPlay()
        {
            while (this.Board.State == ChessGameState.Playing || this.Board.State == ChessGameState.Check)
            {
                this.BeforeMove?.Invoke(this, EventArgs.Empty);

                DateTime start = DateTime.Now;
                ChessMove move = this.CurrentPlayer.Move(this.Board);
                this.LastTurnDuration = DateTime.Now - start;

                this.Board.ExecuteMove(move);

                this.AfterMove?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool TakeTurn()
        {
            this.BeforeMove?.Invoke(this, EventArgs.Empty);

            DateTime start = DateTime.Now;
            ChessMove move = this.CurrentPlayer.Move(this.Board);
            this.LastTurnDuration = DateTime.Now - start;

            this.Board.ExecuteMove(move);

            this.AfterMove?.Invoke(this, EventArgs.Empty);

            return this.Board.State == ChessGameState.Playing || this.Board.State == ChessGameState.Check;
        }

    }
}
