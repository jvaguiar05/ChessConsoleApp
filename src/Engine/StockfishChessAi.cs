using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;

namespace ChessConsoleApp.Engine;

public sealed class StockfishChessAi(StockfishUciClient client) : IDisposable
{
    private readonly StockfishUciClient _client = client;

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

        string fen = FenConverter.ToFen(session);
        int elo = GetElo(difficulty);
        int moveTimeMs = GetMoveTimeMs(difficulty);

        string? bestMove = _client.GetBestMove(fen, elo, moveTimeMs);

        if (string.IsNullOrWhiteSpace(bestMove))
            return false;

        if (!TryParseUciMove(bestMove, out from, out to))
            return false;

        return true;
    }

    private static bool TryParseUciMove(string uciMove, out Position from, out Position to)
    {
        from = default;
        to = default;

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
            AiDifficulty.Intermediate => 1600,
            AiDifficulty.Advanced => 2000,
            AiDifficulty.Master => 2400,
            AiDifficulty.Grandmaster => 3000,
            _ => 1600,
        };
    }

    private static int GetMoveTimeMs(AiDifficulty difficulty)
    {
        return difficulty switch
        {
            AiDifficulty.Beginner => 100,
            AiDifficulty.Intermediate => 300,
            AiDifficulty.Advanced => 700,
            AiDifficulty.Master => 1200,
            AiDifficulty.Grandmaster => 2000,
            _ => 300,
        };
    }

    public void Dispose() => _client.Dispose();
}
