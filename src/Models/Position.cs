namespace ChessGame.Models;

public readonly struct Position(int row, int column)
{
    public int Row { get; init; } = row;
    public int Column { get; init; } = column;

    public bool IsOnBoard => Row is >= 0 and < 8 && Column is >= 0 and < 8;

    public static Position FromAlgebraic(string notation)
    {
        if (notation.Length != 2)
            throw new ArgumentException("Invalid notation length.");

        int col = notation[0] - 'a'; // 'a' -> 0, 'h' -> 7
        int row = notation[1] - '1'; // '1' -> 0, '8' -> 7

        return new Position(row, col);
    }

    public override string ToString()
    {
        char file = (char)('a' + Column);
        int rank = Row + 1; // 0 -> 1, 7 -> 8
        return $"{file}{rank}";
    }
}
