using ChessGame.Core;
using ChessGame.Enums;
using ChessGame.Models;
using ChessGame.Models.Pieces;

namespace ChessConsoleApp.Tests.BoardTests;

public static class BoardTestHelpers
{
    public static Board CreateInitializedBoard()
    {
        Board board = new Board();
        board.Initialize();

        return board;
    }

    public static bool HasPiece<TPiece>(Board board, int row, int col, PieceColor color)
        where TPiece : Piece
    {
        return board[new Position(row, col)] is TPiece piece && piece.Color == color;
    }

    public static bool HasBackRank(Board board, int row, PieceColor color)
    {
        return HasPiece<Rook>(board, row, 0, color)
            && HasPiece<Knight>(board, row, 1, color)
            && HasPiece<Bishop>(board, row, 2, color)
            && HasPiece<Queen>(board, row, 3, color)
            && HasPiece<King>(board, row, 4, color)
            && HasPiece<Bishop>(board, row, 5, color)
            && HasPiece<Knight>(board, row, 6, color)
            && HasPiece<Rook>(board, row, 7, color);
    }

    public static bool PrintResult(bool passed, string message)
    {
        Console.WriteLine(passed ? $" [PASS] {message}" : $" [FAIL] {message}");
        return passed;
    }
}
