using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;
using static ChessConsoleApp.Tests.GameStateTests.GameStateTestHelpers;

namespace ChessConsoleApp.Tests.GameStateTests;

public static class StateTransitionTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= Checkmate_ShouldTransitionToCheckmate();
        allPassed &= Stalemate_ShouldTransitionToStalemate();
        allPassed &= RunningGame_ShouldRemainRunningAfterNormalMove();

        return allPassed;
    }

    public static bool Checkmate_ShouldTransitionToCheckmate()
    {
        var engine = CreateEngineWith(
            new King(PieceColor.White, new Position(0, 7)), // h1
            new King(PieceColor.Black, new Position(7, 7)), // h8
            new Queen(PieceColor.Black, new Position(1, 6)), // g2
            new Rook(PieceColor.Black, new Position(2, 6)) // g3 protects queen
        );

        engine.SetCurrentTurnForTesting(PieceColor.White);

        return ExpectState(
            engine.EvaluateGameStateForTesting(),
            GameState.Checkmate,
            "Checkmate position transitions to Checkmate."
        );
    }

    public static bool Stalemate_ShouldTransitionToStalemate()
    {
        var engine = CreateEngineWith(
            new King(PieceColor.Black, new Position(7, 7)), // h8
            new King(PieceColor.White, new Position(6, 5)), // f7
            new Queen(PieceColor.White, new Position(5, 6)) // g6
        );

        engine.SetCurrentTurnForTesting(PieceColor.Black);

        return ExpectState(
            engine.EvaluateGameStateForTesting(),
            GameState.Stalemate,
            "Stalemate position transitions to Stalemate."
        );
    }

    public static bool RunningGame_ShouldRemainRunningAfterNormalMove()
    {
        var engine = CreateEngine();

        GameState state = engine.TestMoveInput("e2 e4");

        return ExpectState(state, GameState.Running, "Normal legal move keeps game running.");
    }
}
