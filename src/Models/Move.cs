using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Models;

public readonly struct Move(
    Position from,
    Position to,
    Piece pieceMoved,
    Piece? pieceCaptured = null
)
{
    public Position From { get; } = from;
    public Position To { get; } = to;
    public Piece PieceMoved { get; } = pieceMoved;
    public Piece? PieceCaptured { get; } = pieceCaptured;

    // Helper property to determine if this move was a pawn's initial double-step sprint
    public bool IsPawnDoubleAdvance => PieceMoved is Pawn && Math.Abs(From.Row - To.Row) == 2;
}
