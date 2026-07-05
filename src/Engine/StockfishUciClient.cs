using System.Diagnostics;

namespace ChessConsoleApp.Engine;

public sealed class StockfishUciClient(StockfishOptions options) : IDisposable
{
    private readonly StockfishOptions _options = options;
    private Process? _process;
    private StreamWriter? _input;
    private StreamReader? _output;
    private bool _isInitialized;

    public string? GetBestMove(string fen, int elo, int moveTimeMs)
    {
        EnsureInitialized();

        ConfigureStrength(elo);
        WaitUntilReady();

        SendCommand($"position fen {fen}");
        SendCommand($"go movetime {moveTimeMs}");

        string? bestMoveLine = ReadUntil(
            line => line.StartsWith("bestmove", StringComparison.OrdinalIgnoreCase),
            _options.MoveTimeoutMs
        );

        if (string.IsNullOrWhiteSpace(bestMoveLine))
            return null;

        string[] parts = bestMoveLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            return null;

        string bestMove = parts[1];

        if (bestMove == "(none)")
            return null;

        return bestMove;
    }

    private void EnsureInitialized()
    {
        if (_isInitialized)
            return;

        StartProcess();

        SendCommand("uci");

        string? uciOk =
            ReadUntil(
                line => line.Equals("uciok", StringComparison.OrdinalIgnoreCase),
                _options.StartupTimeoutMs
            ) ?? throw new InvalidOperationException("Stockfish did not respond with 'uciok'.");

        WaitUntilReady();

        SendCommand("ucinewgame");
        WaitUntilReady();

        _isInitialized = true;
    }

    private void StartProcess()
    {
        string executablePath = ResolveExecutablePath();

        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        _process = Process.Start(startInfo);

        if (_process == null)
            throw new InvalidOperationException("Failed to start Stockfish process.");

        _input = _process.StandardInput;
        _output = _process.StandardOutput;
    }

    private string ResolveExecutablePath()
    {
        if (File.Exists(_options.ExecutablePath))
            return _options.ExecutablePath;

        string fromBaseDirectory = Path.Combine(AppContext.BaseDirectory, _options.ExecutablePath);

        if (File.Exists(fromBaseDirectory))
            return fromBaseDirectory;

        throw new FileNotFoundException(
            $"Stockfish executable was not found at '{_options.ExecutablePath}'."
        );
    }

    private void ConfigureStrength(int elo)
    {
        SendCommand("setoption name UCI_LimitStrength value true");
        SendCommand($"setoption name UCI_Elo value {elo}");
    }

    private void WaitUntilReady()
    {
        SendCommand("isready");

        string? readyOk =
            ReadUntil(
                line => line.Equals("readyok", StringComparison.OrdinalIgnoreCase),
                _options.ReadyTimeoutMs
            ) ?? throw new InvalidOperationException("Stockfish did not respond with 'readyok'.");
    }

    private void SendCommand(string command)
    {
        if (_input == null)
            throw new InvalidOperationException("Stockfish input stream is not available.");

        _input.WriteLine(command);
        _input.Flush();
    }

    private string? ReadUntil(Func<string, bool> predicate, int timeoutMs)
    {
        if (_output == null)
            throw new InvalidOperationException("Stockfish output stream is not available.");

        DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);

        while (DateTime.UtcNow < deadline)
        {
            string? line = _output.ReadLine();

            if (line == null)
                continue;

            if (predicate(line))
                return line;
        }

        return null;
    }

    public void Dispose()
    {
        try
        {
            if (_process is { HasExited: false })
            {
                SendCommand("quit");
                _process.WaitForExit(1000);
            }
        }
        catch
        {
            // Ignore cleanup failures.
        }
        finally
        {
            _input?.Dispose();
            _output?.Dispose();
            _process?.Dispose();
        }
    }
}
