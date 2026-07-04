using ChessGame.Models;

namespace ChessConsoleApp.UI;

public static class InputParser
{
    public static bool TryParseMove(string input, out Position from, out Position to)
    {
        from = default;
        to = default;

        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return false;

        try
        {
            from = Position.FromAlgebraic(parts[0]);
            to = Position.FromAlgebraic(parts[1]);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
