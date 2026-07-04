using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;

namespace ChessConsoleApp.Models.Pieces;

public class Queen(PieceColor color, Position position) : Piece(color, position)
{
    public override bool IsValidMove(Board board, Position destination, Move? lastMove = null)
    {
        // Delegate verification to the Rook and Bishop implementations
        var horizontalVerticalCheck = new Rook(Color, Position);
        var diagonalCheck = new Bishop(Color, Position);

        return horizontalVerticalCheck.IsValidMove(board, destination)
            || diagonalCheck.IsValidMove(board, destination);
    }
}
