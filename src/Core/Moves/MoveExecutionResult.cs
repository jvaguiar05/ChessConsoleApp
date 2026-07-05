using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Core.Moves;

public sealed class MoveExecutionResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }

    public Piece? MovedPiece { get; init; }
    public Position PromotionPosition { get; init; }
    public bool RequiresPromotion { get; init; }

    public static MoveExecutionResult Success(
        Piece movedPiece,
        Position promotionPosition,
        bool requiresPromotion
    )
    {
        return new MoveExecutionResult
        {
            Succeeded = true,
            MovedPiece = movedPiece,
            PromotionPosition = promotionPosition,
            RequiresPromotion = requiresPromotion,
        };
    }

    public static MoveExecutionResult Failed(string errorMessage) =>
        new() { Succeeded = false, ErrorMessage = errorMessage };
}
