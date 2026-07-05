using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;

namespace ChessConsoleApp.Core;

public sealed class GameStateEvaluator(Func<Position, Position, bool> isMoveLegal)
{
    private readonly Func<Position, Position, bool> _isMoveLegal = isMoveLegal;

    public GameState Evaluate(GameSession session)
    {
        Move? lastMove = session.LastMove;

        if (session.HalfMoveClock >= 100)
            return session.State = GameState.DrawByFiftyMoveRule;

        string currentSnapshot = PositionSnapshotService.Generate(session);

        if (
            session.PositionHistory.TryGetValue(currentSnapshot, out int repetitionCount)
            && repetitionCount >= 3
        )
            return session.State = GameState.DrawByRepetition;

        if (session.Board.HasInsufficientMaterial())
            return session.State = GameState.DrawByInsufficientMaterial;

        bool inCheck = session.Board.IsColorInCheck(session.CurrentTurn, lastMove);

        bool hasLegalMoves = session.Board.HasAnyLegalMoves(
            session.CurrentTurn,
            lastMove,
            _isMoveLegal
        );

        if (!hasLegalMoves)
            return session.State = inCheck ? GameState.Checkmate : GameState.Stalemate;

        return session.State;
    }
}
