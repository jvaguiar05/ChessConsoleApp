using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;

namespace ChessConsoleApp.Engine;

public sealed class StockfishChessAi(StockfishUciClient client) : IDisposable
{
    private readonly StockfishUciClient _client = client;
    public char? LastPromotionChoice { get; private set; }

    public StockfishChessAi()
        : this(new StockfishUciClient(new StockfishOptions())) { }

    public bool TryGetBestMove(
        GameSession session,
        AiDifficulty difficulty,
        out Position from,
        out Position to
    )
    {
        from = default;
        to = default;
        LastPromotionChoice = null;

        string fen = FenConverter.ToFen(session);
        int elo = GetElo(difficulty);
        int moveTimeMs = GetMoveTimeMs(difficulty);

        string? bestMove = _client.GetBestMove(fen, elo, moveTimeMs);

        if (string.IsNullOrWhiteSpace(bestMove))
            return false;

        if (!TryParseUciMove(bestMove, out from, out to, out char? promotionChoice))
            return false;

        LastPromotionChoice = promotionChoice;

        return true;
    }

    private static bool TryParseUciMove(
        string uciMove,
        out Position from,
        out Position to,
        out char? promotionChoice
    )
    {
        from = default;
        to = default;
        promotionChoice = null;

        if (uciMove.Length < 4)
            return false;

        string fromNotation = uciMove[..2];
        string toNotation = uciMove[2..4];

        try
        {
            from = Position.FromAlgebraic(fromNotation);
            to = Position.FromAlgebraic(toNotation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static int GetElo(AiDifficulty difficulty)
    {
        return difficulty switch
        {
            AiDifficulty.Beginner => 1320,
            AiDifficulty.Intermediate => 1500,
            AiDifficulty.Advanced => 1800,
            AiDifficulty.Master => 2200,
            AiDifficulty.Grandmaster => 3200,
            _ => 1500,
        };
    }

    private static int GetMoveTimeMs(AiDifficulty difficulty)
    {
        return difficulty switch
        {
            AiDifficulty.Beginner => 25,
            AiDifficulty.Intermediate => 150,
            AiDifficulty.Advanced => 500,
            AiDifficulty.Master => 1200,
            AiDifficulty.Grandmaster => 5000,
            _ => 150,
        };
    }

    public void Dispose() => _client.Dispose();
}
