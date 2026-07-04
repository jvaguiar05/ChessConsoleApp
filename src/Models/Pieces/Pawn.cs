using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;

namespace ChessConsoleApp.Models.Pieces;

public class Pawn(PieceColor color, Position position) : Piece(color, position)
{
    public override bool IsValidMove(Board board, Position destination, Move? lastMove = null)
    {
        if (!destination.IsOnBoard)
            return false;

        int forwardStep = Color == PieceColor.White ? 1 : -1;
        int startingRow = Color == PieceColor.White ? 1 : 6;

        int rowDelta = destination.Row - Position.Row;
        int colDelta = Math.Abs(destination.Column - Position.Column);

        // Single-step forward advance
        if (rowDelta == forwardStep && colDelta == 0)
            return board.IsSquareEmpty(destination);

        // Two-step initial forward advance
        if (rowDelta == forwardStep * 2 && colDelta == 0 && Position.Row == startingRow)
        {
            var intermediateSquare = new Position(Position.Row + forwardStep, Position.Column);
            return board.IsSquareEmpty(intermediateSquare) && board.IsSquareEmpty(destination);
        }

        // Standard diagonal capture or En Passant
        if (rowDelta == forwardStep && colDelta == 1)
        {
            if (board.IsEnemyPiece(destination, Color))
                return true;

            // En Passant validation rule checks
            if (lastMove.HasValue && lastMove.Value.IsPawnDoubleAdvance)
            {
                Position enemyPawnPos = lastMove.Value.To;
                return enemyPawnPos.Row == Position.Row
                    && enemyPawnPos.Column == destination.Column;
            }
        }

        return false;
    }
}
