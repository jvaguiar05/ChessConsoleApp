using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.UI;

public static class ConsoleRenderer
{
    private static readonly Dictionary<string, (int Value, char Symbol)> PieceMetrics =
        new()
        {
            { "Pawn", (1, '♟') },
            { "Knight", (3, '♞') },
            { "Bishop", (3, '♝') },
            { "Rook", (5, '♜') },
            { "Queen", (9, '♛') },
        };

    public static void RenderBoard(Board board, List<Move>? moveHistory = null)
    {
        Console.Clear();
        Console.WriteLine("    a   b   c   d   e   f   g   h");
        Console.WriteLine("  +-------------------------------+");

        var capturedByWhite = new List<char>();
        var capturedByBlack = new List<char>();
        int materialBalance = 0;

        if (moveHistory != null)
        {
            foreach (var move in moveHistory)
            {
                if (move.PieceCaptured != null)
                {
                    string name = move.PieceCaptured.GetType().Name;
                    if (PieceMetrics.TryGetValue(name, out var metrics))
                    {
                        if (move.PieceCaptured.Color == PieceColor.Black)
                        {
                            capturedByWhite.Add(metrics.Symbol);
                            materialBalance += metrics.Value;
                        }
                        else
                        {
                            capturedByBlack.Add(metrics.Symbol);
                            materialBalance -= metrics.Value;
                        }
                    }
                }
            }
        }

        for (int r = 7; r >= 0; r--)
        {
            Console.Write($"{r + 1} |");

            for (int c = 0; c < 8; c++)
            {
                Piece? piece = board[new Position(r, c)];

                if (piece != null)
                {
                    Console.ForegroundColor =
                        piece.Color == PieceColor.White
                            ? ConsoleColor.White
                            : ConsoleColor.DarkGray;
                }

                Console.Write($" {GetPieceSymbol(piece)} ");
                Console.ResetColor();
                Console.Write("|");
            }

            Console.Write($" {r + 1}");

            // Append right side text components cleanly matching row rows
            AppendSidebarContent(r, capturedByWhite, capturedByBlack, materialBalance);
            Console.WriteLine();

            // Simple board horizontal rules without injection offsets to fix the image_5a099e.png drift
            Console.WriteLine("  +-------------------------------+");
        }

        Console.WriteLine("    a   b   c   d   e   f   g   h\n");
    }

    private static void AppendSidebarContent(
        int row,
        List<char> whiteTrophies,
        List<char> blackTrophies,
        int score
    )
    {
        switch (row)
        {
            case 7:
                Console.Write("    === MATCH STATS ===");
                break;
            case 6:
                Console.Write("    -------------------");
                break;
            case 5:
                string whiteStr =
                    whiteTrophies.Count > 0 ? string.Join(" ", whiteTrophies) : "(None)";
                Console.Write($"    Captured by White: {whiteStr}");
                break;
            case 4:
                string blackStr =
                    blackTrophies.Count > 0 ? string.Join(" ", blackTrophies) : "(None)";
                Console.Write($"    Captured by Black: {blackStr}");
                break;
            case 3:
                Console.Write("    -------------------");
                break;
            case 2:
                Console.Write("    Material Balance:  ");
                if (score > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"+{score} White Advantage");
                }
                else if (score < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{score} Black Advantage");
                }
                else
                {
                    Console.Write("Even");
                }
                Console.ResetColor();
                break;
        }
    }

    private static char GetPieceSymbol(Piece? piece)
    {
        if (piece == null)
            return '·';

        return piece.GetType().Name switch
        {
            "Pawn" => 'P',
            "Rook" => 'R',
            "Knight" => 'N',
            "Bishop" => 'B',
            "Queen" => 'Q',
            "King" => 'K',
            _ => '?',
        };
    }
}
