using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;

namespace ChessConsoleApp.Models.Pieces;

public class Rook(PieceColor color, Position position) : Piece(color, position)
{
    public override bool IsValidMove(Board board, Position destination, Move? lastMove = null)
    {
        if (!destination.IsOnBoard)
            return false;

        int rowDelta = Math.Abs(Position.Row - destination.Row);
        int colDelta = Math.Abs(Position.Column - destination.Column);

        bool isStraightLine = (rowDelta == 0 && colDelta > 0) || (colDelta == 0 && rowDelta > 0);
        if (!isStraightLine)
            return false;

        int rowStep = Math.Sign(destination.Row - Position.Row);
        int colStep = Math.Sign(destination.Column - Position.Column);

        int currentCheckRow = Position.Row + rowStep;
        int currentCheckCol = Position.Column + colStep;

        // Scan intermediary squares along the path for obstacles
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
