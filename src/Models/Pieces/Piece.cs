using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;

namespace ChessConsoleApp.Models.Pieces;

public abstract class Piece
{
    public PieceColor Color { get; }
    public Position Position { get; private set; }

    protected Piece(PieceColor color, Position position)
    {
        Color = color;
        Position = position;
    }

    // Should only be invoked by the Board to synchronize state during a move
    public void SetPosition(Position newPosition) => Position = newPosition;

    public abstract bool IsValidMove(Board board, Position destination, Move? lastMove = null);

    public bool HasMoved(List<Move> moveHistory) =>
        moveHistory.Any(move => move.PieceMoved == this);
}
