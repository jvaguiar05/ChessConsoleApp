using ChessGame.Core;
using ChessGame.Enums;

namespace ChessGame.Models.Pieces;

public class Knight(PieceColor color, Position position) : Piece(color, position)
{
    public override bool IsValidMove(Board board, Position destination, Move? lastMove = null)
    {
        if (!destination.IsOnBoard)
            return false;

        int rowDelta = Math.Abs(Position.Row - destination.Row);
        int colDelta = Math.Abs(Position.Column - destination.Column);

        // Verify geometric L-shape constraint
        bool isValidLShape = (rowDelta == 2 && colDelta == 1) || (rowDelta == 1 && colDelta == 2);
        if (!isValidLShape)
            return false;

        return board.IsSquareEmpty(destination) || board.IsEnemyPiece(destination, Color);
    }
}
