using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Core.Moves;

public static class PromotionPieceFactory
{
    public static Piece Create(char choice, PieceColor color, Position position)
    {
        return char.ToUpper(choice) switch
        {
            'R' => new Rook(color, position),
            'B' => new Bishop(color, position),
            'N' => new Knight(color, position),
            _ => new Queen(color, position),
        };
    }
}
