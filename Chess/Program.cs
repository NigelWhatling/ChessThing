namespace Chess
{
    using System;
    using System.Threading;
    using Chess.Engine;

    public class Program
    {
        public static void Main(string[] args)
        {
            ChessGame game = new ChessGame();
            game.Board.DumpBoard(false);

            Console.WriteLine();
            Console.WriteLine($"{game.Turn} {game.TurnColour}");

            game.BeforeMove += (s, e) =>
            {
                Console.Clear();
                game.Board.DumpBoard(false);

                Console.WriteLine();
                Console.WriteLine($"{game.Turn} {game.TurnColour} {game.Board.State} {game.LastTurnDuration}");

                Thread.Sleep(20);
                //Console.ReadKey();
            };

            Console.ReadKey();
            game.AutoPlay();

            //Console.Clear();
            game.Board.DumpBoard(false);

            Console.WriteLine();
            Console.WriteLine($"{game.Turn} {game.TurnColour} {game.Board.State}");

            Console.ReadKey();

        }
    }
}
