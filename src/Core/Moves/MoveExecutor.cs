using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Core.Moves;

public sealed class MoveExecutor(MoveValidator moveValidator)
{
    private readonly MoveValidator _moveValidator = moveValidator;

    public MoveExecutionResult Execute(GameSession session, Position from, Position to)
    {
        MoveValidationResult validation = _moveValidator.Validate(session, from, to);

        if (!validation.IsLegal)
            return MoveExecutionResult.Failed(validation.ErrorMessage ?? "Invalid move.");

        Piece piece = validation.MovingPiece!;

        session.MoveHistory.Add(new Move(from, to, piece, validation.CapturedPiece));
        session.Board.MovePiece(from, to);

        if (piece is Pawn || validation.CapturedPiece != null)
            session.HalfMoveClock = 0;
        else
            session.HalfMoveClock++;

        if (validation.IsCastling && validation.CastlingRook != null)
            session.Board.MovePiece(validation.RookStartPosition, validation.RookTransitPosition);

        bool requiresPromotion = piece is Pawn && (to.Row == 0 || to.Row == 7);

        return MoveExecutionResult.Success(piece, to, requiresPromotion);
    }
}
