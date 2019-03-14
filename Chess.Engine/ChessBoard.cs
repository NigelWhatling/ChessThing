namespace Chess.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ChessBoard
    {
        public ChessGame Game { get; }

        public int Turn { get; private set; }

        private int TurnPlayerIndex => (this.Turn + 1) % 2;

        public ChessPlayerColour TurnColour => this.Game.Players[this.TurnPlayerIndex].Colour;

        public IList<ChessPiece> Pieces { get; }

        public IEnumerable<ChessPiece> ActivePieces => this.Pieces.Where(p => p.IsOnBoard);

        public IEnumerable<ChessPiece> ActiveCurrentPlayerPieces => this.ActivePieces.Where(p => p.Colour == this.TurnColour);

        public IEnumerable<ChessPiece> ActiveOpponentPieces => this.ActivePieces.Where(p => p.Colour != this.TurnColour);

        public ChessGameState State { get; private set; }

        private readonly ChessBoardSquare[,] _boardState = new ChessBoardSquare[8, 8];

        public int FullMoves => this.Turn / 2;

        private int HalfMovesSinceCaptureOrPawn = 0;

        private ChessBoardStateFlags StateFlags = ChessBoardStateFlags.None;

        public bool IsClone { get; }

        public string ExecutingMove { get; private set; }

        private ChessBoard()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    this._boardState[x, y] = new ChessBoardSquare(x, y);
                }
            }
        }

        public ChessBoard(ChessGame game) : this()
        {
            this.Game = game;
            this.Turn = 1;
            this.HalfMovesSinceCaptureOrPawn = 0;
            this.Pieces = new List<ChessPiece>(32);
            this.State = ChessGameState.Playing;
            this.Reset();
        }

        private ChessBoard(ChessBoard board, int? turn = null) : this()
        {
            this.IsClone = true;
            this.Game = board.Game;
            this.Turn = turn ?? board.Turn;
            this.HalfMovesSinceCaptureOrPawn = board.HalfMovesSinceCaptureOrPawn;
            this.Pieces = board.Pieces.Select(p => p.Clone(this)).ToList();
            this.State = board.State;
            this.StateFlags = board.StateFlags;
            this.UpdateBoardState();
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
            this.Turn = 1;
            this.HalfMovesSinceCaptureOrPawn = 0;
            this.State = ChessGameState.Playing;
            this.StateFlags = ChessBoardStateFlags.WhiteCastleKingside | ChessBoardStateFlags.WhiteCastleQueenside | ChessBoardStateFlags.BlackCastleKingside | ChessBoardStateFlags.BlackCastleQueenside;
            this.Pieces.Clear();
            this.ResetPlayerPieces(this.Game.Players[0].Colour, 1);
            this.ResetPlayerPieces(this.Game.Players[1].Colour, -1);
            this.UpdateBoardState();
        }

        private void ResetPlayerPieces(ChessPlayerColour colour, int direction)
        {
            int baseY = direction > 0 ? 0 : 7;

            this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.King, new ChessLocation(4, baseY), direction));
            this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.Queen, new ChessLocation(3, baseY), direction));
            this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.Bishop, 1, new ChessLocation(2, baseY), direction));
            this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.Bishop, 2, new ChessLocation(5, baseY), direction));
            this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.Knight, 1, new ChessLocation(1, baseY), direction));
            this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.Knight, 2, new ChessLocation(6, baseY), direction));
            this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.Rook, 1, new ChessLocation(0, baseY), direction));
            this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.Rook, 2, new ChessLocation(7, baseY), direction));

            for (int i = 0; i <= 7; i++)
            {
                this.Pieces.Add(new ChessPiece(this, colour, ChessPieceType.Pawn, i + 1, new ChessLocation(i, baseY + direction), direction));
            }
        }

        public ChessBoard Clone()
        {
            ChessBoard board = new ChessBoard(this);
            return board;
        }

        public ChessPiece GetPieceFromLocation(int x, int y) => this.ActivePieces.Where(p => p.Location == new ChessLocation(x, y)).SingleOrDefault();

        public ChessPiece GetPieceFromLocation(ChessLocation location) => this.ActivePieces.Where(p => p.Location == location).SingleOrDefault();

        public void ExecuteMove(ChessMove move)
        {
            this.ExecutingMove = null;

            if (!move.IsValid)
            {
                return;
            }

            if (!this.ValidateMove(move, out ChessPiece movingPiece, out ChessPiece targetPiece))
            {
                return;
            }

            this.HalfMovesSinceCaptureOrPawn++;
            if (movingPiece.PieceType == ChessPieceType.Pawn)
            {
                this.HalfMovesSinceCaptureOrPawn = 0;
            }

            if (targetPiece != null)
            {
                targetPiece.Capture();
                this.HalfMovesSinceCaptureOrPawn = 0;
            }

            if (movingPiece.PieceType == ChessPieceType.King)
            {
                this.StateFlags &= ~(movingPiece.Colour == ChessPlayerColour.White ? ChessBoardStateFlags.WhiteCastleKingside : ChessBoardStateFlags.BlackCastleKingside);
                this.StateFlags &= ~(movingPiece.Colour == ChessPlayerColour.White ? ChessBoardStateFlags.WhiteCastleQueenside : ChessBoardStateFlags.BlackCastleQueenside);
            }
            else if (movingPiece.PieceType == ChessPieceType.Rook)
            {
                if (movingPiece.Location.X == 0)
                {
                    this.StateFlags &= ~(movingPiece.Colour == ChessPlayerColour.White ? ChessBoardStateFlags.WhiteCastleQueenside : ChessBoardStateFlags.BlackCastleQueenside);
                }
                else if (movingPiece.Location.X == 7)
                {
                    this.StateFlags &= ~(movingPiece.Colour == ChessPlayerColour.White ? ChessBoardStateFlags.WhiteCastleKingside : ChessBoardStateFlags.BlackCastleKingside);
                }
            }

            movingPiece.Move(move.to);

            if (move.promoteTo.HasValue)
            {
                movingPiece.Promote(move.promoteTo.Value);
            }
            else if (move.castleFrom.HasValue)
            {
                this.GetPieceFromLocation(move.castleFrom.Value).Move(move.castleTo.Value);
            }
            else if (move.targetLocation.HasValue)
            {
                this.GetPieceFromLocation(move.targetLocation.Value).Capture();
                this.HalfMovesSinceCaptureOrPawn = 0;
            }

            if (!this.IsClone)
            {
                this.Turn++;
            }

            this.UpdateBoardState();
        }

        public bool ValidateMove(ChessMove move, out ChessPiece movingPiece, out ChessPiece targetPiece)
        {
            movingPiece = this.GetPieceFromLocation(move.from);
            targetPiece = this.GetPieceFromLocation(move.to);

            this.ExecutingMove = $"{move} {movingPiece?.PieceType} - {targetPiece?.PieceType}"; 

            if (movingPiece == null)
            {
                throw new InvalidOperationException("Tried to move piece from vacant square.");
            }
            
            if (targetPiece != null)
            {
                if (!this.IsClone && targetPiece.PieceType == ChessPieceType.King)
                {
                    throw new InvalidOperationException("Tried to capture a king!");
                }
            }

            return true;
        }

        public ChessBoard TestMove(ChessMove move)
        {
            ChessBoard board = this.Clone();
            board.ExecuteMove(move);
            return board;
        }

        private const byte PieceIsOnBoard = 0x40;
        private const byte PieceHasMoved = 0x80;

        public string CreateBoardSnapshot()
        {
            /*
             * |---------------32---------------|-|-|--|--|
             * 
             *  0 - 31 : Square by square, 4 bits per square, piece type
             *      32 : Board state flags
             *      33 : En passant location
             * 34 - 35 : Half move count since last capture/pawn
             * 36 - 37 : Move count
             * 
             * Total: 32 + 1 + 1 + 2 + 2 = 38 bytes
             */

            List<byte> snapshot = new List<byte>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x += 2)
                {
                    byte block = 0;

                    ChessPiece piece = this[x, y].OccupiedBy;
                    if (piece != null)
                    {
                        block |= (byte)piece.PieceType;
                        if (piece.Colour == ChessPlayerColour.Black)
                        {
                            block |= 0x08;
                        }
                    }

                    block <<= 4;

                    piece = this[x + 1, y].OccupiedBy;
                    if (piece != null)
                    {
                        block |= (byte)piece.PieceType;
                        if (piece.Colour == ChessPlayerColour.Black)
                        {
                            block |= 0x08;
                        }
                    }

                    snapshot.Add(block);
                }
            }

            snapshot.Add((byte)this.StateFlags);

            // TODO: En passant location
            snapshot.Add(0);

            // TODO: Half move count
            snapshot.AddRange(BitConverter.GetBytes((short)this.HalfMovesSinceCaptureOrPawn));

            snapshot.AddRange(BitConverter.GetBytes((short)this.FullMoves));

            //return BitConverter.ToString(snapshot.ToArray());
            return Convert.ToBase64String(snapshot.ToArray());
        }

        [Flags]
        public enum ChessBoardStateFlags
        {
            None                    = 0,
            WhiteCastleKingside     = 0x01,
            WhiteCastleQueenside    = 0x02,
            BlackCastleKingside     = 0x04,
            BlackCastleQueenside    = 0x08
        }

        public void UpdateBoardState()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    this._boardState[x, y].Reset();
                }
            }

            this.State = ChessGameState.Playing;

            int playerPieces = 0;
            foreach (ChessPiece piece in this.ActiveCurrentPlayerPieces)
            {
                playerPieces++;
                this._boardState[piece.Location.X, piece.Location.Y].OccupiedBy = piece;
            }

            int opponentPieces = 0;
            foreach (ChessPiece piece in this.ActiveOpponentPieces)
            {
                opponentPieces++;
                this._boardState[piece.Location.X, piece.Location.Y].OccupiedBy = piece;
            }

            if (!this.IsClone)
            {
                if (this.HalfMovesSinceCaptureOrPawn >= 50)
                {
                    this.State = ChessGameState.DrawFiftyMoveRule;
                }

                if (playerPieces == 1 && opponentPieces == 1)
                {
                    this.State = ChessGameState.Draw;
                }
                else if (playerPieces == 1 && opponentPieces == 2)
                {
                    ChessPiece other = this.ActiveOpponentPieces.Where(p => p.PieceType != ChessPieceType.King).Single();
                    if (other.PieceType == ChessPieceType.Bishop || other.PieceType == ChessPieceType.Knight)
                    {
                        this.State = ChessGameState.Draw;
                    }
                }
                else if (opponentPieces == 1 && playerPieces == 2)
                {
                    ChessPiece other = this.ActiveCurrentPlayerPieces.Where(p => p.PieceType != ChessPieceType.King).Single();
                    if (other.PieceType == ChessPieceType.Bishop || other.PieceType == ChessPieceType.Knight)
                    {
                        this.State = ChessGameState.Draw;
                    }
                }
                else if (opponentPieces == 2 && playerPieces == 2)
                {
                    ChessPiece other1 = this.ActiveCurrentPlayerPieces.Where(p => p.PieceType != ChessPieceType.King).Single();
                    ChessPiece other2 = this.ActiveOpponentPieces.Where(p => p.PieceType != ChessPieceType.King).Single();
                    if (other1.PieceType == ChessPieceType.Bishop && other2.PieceType == ChessPieceType.Bishop
                        && other1.Location.IsLightSquare == other2.Location.IsLightSquare)
                    {
                        this.State = ChessGameState.Draw;
                    }
                }

                if (this.State != ChessGameState.Playing)
                {
                    return;
                }
            }

            if (!this.IsClone && this.GetAllCurrentMoves().Count() == 0)
            {
                this.State = ChessGameState.CheckMate;
                return;
            }

            foreach (ChessMove move in this.ActiveOpponentPieces.SelectMany(p => this.GetMoves(p)).Where(m => m.IsAttackingMove))
            {
                this._boardState[move.to.X, move.to.Y].IsUnderAttack = true;

                ChessPiece target = this._boardState[move.to.X, move.to.Y].OccupiedBy;
                if (target != null && target.PieceType == ChessPieceType.King && target.Colour == this.TurnColour)
                {
                    this.State = ChessGameState.Check;
                }
            }
        }

        public IEnumerable<ChessMove> GetAllCurrentMoves()
        {
            return this.ActiveCurrentPlayerPieces.SelectMany(p => this.GetMoves(p));
        }

        public IEnumerable<ChessMove> GetMoves(ChessPiece piece)
        {
            int x = piece.Location.X;
            int y = piece.Location.Y;
            int direction = piece.Direction;

            List<ChessMove> moves = new List<ChessMove>();

            switch (piece.PieceType)
            {
                case ChessPieceType.King:
                    if (!piece.HasMoved)
                    {
                        foreach (ChessPiece rook in this.ActiveCurrentPlayerPieces.Where(p => p.PieceType == ChessPieceType.Rook && !p.HasMoved))
                        {
                            if (rook.Location.X < x)
                            {
                                bool ok = true;
                                for (int i = x - 1; i >= rook.Location.X; i--)
                                {
                                    if (this[i, y].IsOccupied)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }

                                if (!ok)
                                {
                                    continue;
                                }

                                ChessMove move = new ChessMove(piece.Location, new ChessLocation(x - 2, y));
                                move.castleFrom = rook.Location;
                                move.castleTo = new ChessLocation(x + 3, y);
                            }
                            else if (rook.Location.X > x)
                            {
                                bool ok = true;
                                for (int i = x + 1; i <= rook.Location.X; i++)
                                {
                                    if (this[i, y].IsOccupied)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }

                                if (!ok)
                                {
                                    continue;
                                }

                                ChessMove move = new ChessMove(piece.Location, new ChessLocation(x + 2, y));
                                move.castleFrom = rook.Location;
                                move.castleTo = new ChessLocation(x - 2, y);
                            }
                        }
                    }

                    this.AddMoveRange(moves, x, y, -1, 1, 1);
                    this.AddMoveRange(moves, x, y, 0, 1, 1);
                    this.AddMoveRange(moves, x, y, 1, 1, 1);
                    this.AddMoveRange(moves, x, y, -1, 0, 1);
                    this.AddMoveRange(moves, x, y, 1, 0, 1);
                    this.AddMoveRange(moves, x, y, -1, -1, 1);
                    this.AddMoveRange(moves, x, y, 0, -1, 1);
                    this.AddMoveRange(moves, x, y, 1, -1, 1);
                    break;

                case ChessPieceType.Queen:
                    this.AddMoveRange(moves, x, y, -1, 1, 8);
                    this.AddMoveRange(moves, x, y, 0, 1, 8);
                    this.AddMoveRange(moves, x, y, 1, 1, 8);
                    this.AddMoveRange(moves, x, y, -1, 0, 8);
                    this.AddMoveRange(moves, x, y, 1, 0, 8);
                    this.AddMoveRange(moves, x, y, -1, -1, 8);
                    this.AddMoveRange(moves, x, y, 0, -1, 8);
                    this.AddMoveRange(moves, x, y, 1, -1, 8);
                    break;

                case ChessPieceType.Bishop:
                    this.AddMoveRange(moves, x, y, -1, 1, 8);
                    this.AddMoveRange(moves, x, y, 1, 1, 8);
                    this.AddMoveRange(moves, x, y, -1, -1, 8);
                    this.AddMoveRange(moves, x, y, 1, -1, 8);
                    break;

                case ChessPieceType.Rook:
                    this.AddMoveRange(moves, x, y, 0, 1, 8);
                    this.AddMoveRange(moves, x, y, -1, 0, 8);
                    this.AddMoveRange(moves, x, y, 1, 0, 8);
                    this.AddMoveRange(moves, x, y, 0, -1, 8);
                    break;

                case ChessPieceType.Knight:
                    moves.Add(new ChessMove(piece.Location, new ChessLocation(x - 2, y + 1)));
                    moves.Add(new ChessMove(piece.Location, new ChessLocation(x - 1, y + 2)));
                    moves.Add(new ChessMove(piece.Location, new ChessLocation(x + 1, y + 2)));
                    moves.Add(new ChessMove(piece.Location, new ChessLocation(x + 2, y + 1)));
                    moves.Add(new ChessMove(piece.Location, new ChessLocation(x - 2, y - 1)));
                    moves.Add(new ChessMove(piece.Location, new ChessLocation(x - 1, y - 2)));
                    moves.Add(new ChessMove(piece.Location, new ChessLocation(x + 1, y - 2)));
                    moves.Add(new ChessMove(piece.Location, new ChessLocation(x + 2, y - 1)));
                    break;

                case ChessPieceType.Pawn:
                    if (this[x, y + direction].IsOnBoard && !this[x, y + direction].IsOccupied)
                    {
                        if (!piece.HasMoved && !this[x, y + direction].IsOccupied)
                        {
                            moves.Add(new ChessMove(piece.Location, new ChessLocation(x, y + direction * 2), isAttackingMove: false));
                        }

                        moves.Add(new ChessMove(piece.Location, new ChessLocation(x, y + direction), isAttackingMove: false));
                    }

                    if (this[x - 1, y + direction].OccupiedByColour == piece.OpponentColour)
                    {
                        moves.Add(new ChessMove(piece.Location, new ChessLocation(x - 1, y + direction), isAttackingMove: true));
                    }

                    if (this[x + 1, y + direction].OccupiedByColour == piece.OpponentColour)
                    {
                        moves.Add(new ChessMove(piece.Location, new ChessLocation(x + 1, y + direction), isAttackingMove: true));
                    }

                    if (y >= 3 && y <= 4 && x > 0 && x < 7)
                    {
                        if (this[x - 1, y].IsOccupied)
                        {
                            ChessPiece target = this[x - 1, y].OccupiedBy;
                            if (target.PieceType == ChessPieceType.Pawn && target.JustMoved)
                            {
                                ChessMove move = new ChessMove(piece.Location, new ChessLocation(x - 1, y + direction), isAttackingMove: true);
                                move.targetLocation = new ChessLocation(x - 1, y);
                            }
                        }

                        if (this[x + 1, y].IsOccupied)
                        {
                            ChessPiece target = this[x + 1, y].OccupiedBy;
                            if (target.PieceType == ChessPieceType.Pawn && target.JustMoved)
                            {
                                ChessMove move = new ChessMove(piece.Location, new ChessLocation(x + 1, y + direction), isAttackingMove: true);
                                move.targetLocation = new ChessLocation(x + 1, y);
                            }
                        }
                    }

                    break;
            }

            IEnumerable<ChessMove> allMoves = moves.Where(m => m.to.IsOnBoard && this[m.to].OccupiedByColour != piece.Colour);

            if (!this.IsClone)
            {
                allMoves = allMoves.Where(m => this.TestMove(m).State != ChessGameState.Check);
            }

            return allMoves;
        }

        private void AddMoveRange(List<ChessMove> moves, int x, int y, int xi, int yi, int max)
        {
            ChessLocation startLocation = new ChessLocation(x, y);
            do
            {
                x += xi;
                y += yi;

                ChessLocation location = new ChessLocation(x, y);

                if (!location.IsOnBoard)
                {
                    break;
                }

                moves.Add(new ChessMove(startLocation, location));

                if (this[location].IsOccupied)
                {
                    break;
                }
            } while (--max > 0);
        }

        public void DumpBoard(bool useSymbols)
        {
            this.UpdateBoardState();

            ConsoleColor[,] colours = new ConsoleColor[8, 8];
            char[,] spaces = new char[8, 8];

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    colours[x, y] = ConsoleColor.DarkBlue;
                    spaces[x, y] = ' ';
                }
            }

            foreach (ChessPiece piece in this.ActivePieces)
            {
                colours[piece.Location.X, piece.Location.Y] = piece.Colour == ChessPlayerColour.White ? ConsoleColor.White : ConsoleColor.DarkGray;
                spaces[piece.Location.X, piece.Location.Y] = useSymbols ? piece.Symbol : piece.Code;
            }

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("┌───┬───┬───┬───┬───┬───┬───┬───┐");

            for (int y = 7; y >= 0; y--)
            {
                if (y < 7)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("├───┼───┼───┼───┼───┼───┼───┼───┤");
                }

                for (int x = 0; x < 8; x++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write("│");

                    Console.ForegroundColor = useSymbols ? ConsoleColor.White : colours[x, y];
                    Console.BackgroundColor = this[x, y].IsUnderAttack ? ConsoleColor.DarkRed : ConsoleColor.Black;
                    Console.Write(" " + spaces[x, y] + " ");
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("│");
            }

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("└───┴───┴───┴───┴───┴───┴───┴───┘");

            Console.ResetColor();
            Console.WriteLine();

            //if (this.State == ChessGameState.CheckMate)
            //{
            //    Console.WriteLine("CHECKMATE!");
            //    Console.WriteLine();
            //}
            //else if (this.State == ChessGameState.Check)
            //{
            //    Console.WriteLine("CHECK");
            //    Console.WriteLine();
            //}
            //else if (this.State == ChessGameState.Draw)
            //{
            //    Console.WriteLine("DRAW");
            //    Console.WriteLine();
            //}

            Console.WriteLine(this.CreateBoardSnapshot());
        }
    }
}
