using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Core;

public sealed class MoveValidationResult
{
    public bool IsLegal { get; init; }
    public string? ErrorMessage { get; init; }

    public Position From { get; init; }
    public Position To { get; init; }

    public Piece? MovingPiece { get; init; }
    public Piece? CapturedPiece { get; init; }

    public bool IsEnPassant { get; init; }
    public bool IsCastling { get; init; }

    public Position RookStartPosition { get; init; }
    public Position RookTransitPosition { get; init; }
    public Piece? CastlingRook { get; init; }

    public static MoveValidationResult Legal(
        Position from,
        Position to,
        Piece movingPiece,
        Piece? capturedPiece,
        bool isEnPassant,
        bool isCastling,
        Position rookStartPosition,
        Position rookTransitPosition,
        Piece? castlingRook
    )
    {
        return new MoveValidationResult
        {
            IsLegal = true,
            From = from,
            To = to,
            MovingPiece = movingPiece,
            CapturedPiece = capturedPiece,
            IsEnPassant = isEnPassant,
            IsCastling = isCastling,
            RookStartPosition = rookStartPosition,
            RookTransitPosition = rookTransitPosition,
            CastlingRook = castlingRook,
        };
    }

    public static MoveValidationResult Illegal(Position from, Position to, string errorMessage)
    {
        return new MoveValidationResult
        {
            IsLegal = false,
            From = from,
            To = to,
            ErrorMessage = errorMessage,
        };
    }
}
