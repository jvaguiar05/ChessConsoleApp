using ChessGame.Core;
using ChessGame.Enums;

namespace ChessGame.Models.Pieces;

public class King(PieceColor color, Position position) : Piece(color, position)
{
    public override bool IsValidMove(Board board, Position destination, Move? lastMove = null)
    {
        if (!destination.IsOnBoard)
            return false;

        int rowDelta = Math.Abs(Position.Row - destination.Row);
        int colDelta = Math.Abs(Position.Column - destination.Column);

        if (rowDelta <= 1 && colDelta <= 1 && !(rowDelta == 0 && colDelta == 0))
            return board.IsSquareEmpty(destination) || board.IsEnemyPiece(destination, Color);

        int startingRow = Color == PieceColor.White ? 0 : 7;
        if (
            Position.Row == startingRow
            && destination.Row == startingRow
            && Math.Abs(destination.Column - Position.Column) == 2
        )
        {
            int rookSourceCol = destination.Column == 6 ? 7 : 0; // Column 7 for Short Castle, Column 0 for Long Castle
            Piece? rook = board[new Position(startingRow, rookSourceCol)];

            if (rook is not Rook || rook.Color != Color)
                return false;

            // Ensure clear sightlines between King (col 4) and Rook (col 0 or 7)
            int step = destination.Column == 6 ? 1 : -1;
            for (int col = Position.Column + step; col != rookSourceCol; col += step)
                if (!board.IsSquareEmpty(new Position(startingRow, col)))
                    return false;

            return true;
        }

        return false;
    }
}
