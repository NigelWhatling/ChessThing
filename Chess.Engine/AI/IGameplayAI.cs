namespace Chess.Engine.AI
{
    public interface IGameplayAI
    {
        ChessMove NextMove(ChessBoard board);
    }
}
