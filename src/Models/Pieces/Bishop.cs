using ChessGame.Core;
using ChessGame.Enums;

namespace ChessGame.Models.Pieces;

public class Bishop(PieceColor color, Position position) : Piece(color, position)
{
    public override bool IsValidMove(Board board, Position destination, Move? lastMove = null)
    {
        if (!destination.IsOnBoard)
            return false;

        int rowDelta = Math.Abs(Position.Row - destination.Row);
        int colDelta = Math.Abs(Position.Column - destination.Column);

        // Verify diagonal trajectory geometric constraint
        if (rowDelta != colDelta || rowDelta == 0)
            return false;

        int rowStep = Math.Sign(destination.Row - Position.Row);
        int colStep = Math.Sign(destination.Column - Position.Column);

        int currentCheckRow = Position.Row + rowStep;
        int currentCheckCol = Position.Column + colStep;

        // Scan diagonal path for intervening obstacles
        while (currentCheckRow != destination.Row || currentCheckCol != destination.Column)
        {
            if (!board.IsSquareEmpty(new Position(currentCheckRow, currentCheckCol)))
                return false;

            currentCheckRow += rowStep;
            currentCheckCol += colStep;
        }

        return board.IsSquareEmpty(destination) || board.IsEnemyPiece(destination, Color);
    }
}
