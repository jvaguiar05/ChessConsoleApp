using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;

namespace ChessConsoleApp.AI;

public static class BoardEvaluator
{
    private static readonly Dictionary<string, int> PieceValues =
        new()
        {
            { "Pawn", 100 },
            { "Knight", 320 },
            { "Bishop", 330 },
            { "Rook", 500 },
            { "Queen", 900 },
            { "King", 20000 },
        };

    // --- POSITIONAL PIECE-SQUARE TABLES (From White's perspective) ---
    // Higher values encourage the AI to move pieces to those squares.

    private static readonly int[,] PawnTable =
    {
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 50, 50, 50, 50, 50, 50, 50, 50 },
        { 10, 10, 20, 30, 30, 20, 10, 10 },
        { 5, 5, 10, 25, 25, 10, 5, 5 },
        { 0, 0, 0, 20, 20, 0, 0, 0 },
        { 5, -5, -10, 0, 0, -10, -5, 5 },
        { 5, 10, 10, -20, -20, 10, 10, 5 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
    };

    private static readonly int[,] KnightTable =
    {
        { -50, -40, -30, -30, -30, -30, -40, -50 },
        { -40, -20, 0, 0, 0, 0, -20, -40 },
        { -30, 0, 10, 15, 15, 10, 0, -30 },
        { -30, 5, 15, 20, 20, 15, 5, -30 },
        { -30, 0, 15, 20, 20, 15, 0, -30 },
        { -30, 5, 10, 15, 15, 10, 5, -30 },
        { -40, -20, 0, 5, 5, 0, -20, -40 },
        { -50, -40, -30, -30, -30, -30, -40, -50 },
    };

    private static readonly int[,] BishopTable =
    {
        { -20, -10, -10, -10, -10, -10, -10, -20 },
        { -10, 0, 0, 0, 0, 0, 0, -10 },
        { -10, 0, 5, 10, 10, 5, 0, -10 },
        { -10, 5, 5, 10, 10, 5, 5, -10 },
        { -10, 0, 10, 10, 10, 10, 0, -10 },
        { -10, 10, 10, 10, 10, 10, 10, -10 },
        { -10, 5, 0, 0, 0, 0, 5, -10 },
        { -20, -10, -10, -10, -10, -10, -10, -20 },
    };

    private static readonly int[,] RookTable =
    {
        { 0, 0, 0, 5, 5, 0, 0, 0 },
        { 5, 10, 10, 10, 10, 10, 10, 5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { -5, 0, 0, 0, 0, 0, 0, -5 },
        { 0, 0, 0, 5, 5, 0, 0, 0 },
    };

    private static readonly int[,] QueenTable =
    {
        { -20, -10, -10, -5, -5, -10, -10, -20 },
        { -10, 0, 0, 0, 0, 0, 0, -10 },
        { -10, 0, 5, 5, 5, 5, 0, -10 },
        { -5, 0, 5, 5, 5, 5, 0, -5 },
        { 0, 0, 5, 5, 5, 5, 0, 0 },
        { -10, 5, 5, 5, 5, 5, 5, -10 },
        { -10, 0, 5, 0, 0, 5, 0, -10 },
        { -20, -10, -10, -5, -5, -10, -10, -20 },
    };

    private static readonly int[,] KingTable =
    {
        { -30, -40, -40, -50, -50, -40, -40, -30 },
        { -30, -40, -40, -50, -50, -40, -40, -30 },
        { -30, -40, -40, -50, -50, -40, -40, -30 },
        { -30, -40, -40, -50, -50, -40, -40, -30 },
        { -20, -30, -30, -40, -40, -30, -30, -20 },
        { -10, -20, -20, -20, -20, -20, -20, -10 },
        { 20, 20, 0, 0, 0, 0, 20, 20 },
        { 20, 30, 10, 0, 0, 10, 30, 20 },
    };

    public static int Evaluate(Board board)
    {
        int totalScore = 0;

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var piece = board[new Position(r, c)];
                if (piece == null)
                    continue;

                string name = piece.GetType().Name;
                if (PieceValues.TryGetValue(name, out int baseValue))
                {
                    int positionalBonus = GetPositionalBonus(name, piece.Color, r, c);

                    if (piece.Color == PieceColor.White)
                        totalScore += (baseValue + positionalBonus);
                    else
                        totalScore -= (baseValue + positionalBonus);
                }
            }
        }

        return totalScore;
    }

    private static int GetPositionalBonus(string pieceType, PieceColor color, int row, int col)
    {
        // For Black pieces, the board tables must be vertically flipped
        int evalRow = color == PieceColor.White ? row : (7 - row);
        int evalCol = col;

        return pieceType switch
        {
            "Pawn" => PawnTable[evalRow, evalCol],
            "Knight" => KnightTable[evalRow, evalCol],
            "Bishop" => BishopTable[evalRow, evalCol],
            "Rook" => RookTable[evalRow, evalCol],
            "Queen" => QueenTable[evalRow, evalCol],
            "King" => KingTable[evalRow, evalCol],
            _ => 0,
        };
    }
}
