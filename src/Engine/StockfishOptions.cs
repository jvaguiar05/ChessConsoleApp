namespace ChessConsoleApp.Engine;

public sealed class StockfishOptions
{
    public string ExecutablePath { get; init; } =
        Path.Combine("engines", "stockfish", "stockfish.exe");

    public int StartupTimeoutMs { get; init; } = 5000;
    public int ReadyTimeoutMs { get; init; } = 5000;
    public int MoveTimeoutMs { get; init; } = 10000;
}
