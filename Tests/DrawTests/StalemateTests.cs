using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Tests.DrawTests;

public static class StalemateTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= Stalemate_ShouldBeDetected();

        return allPassed;
    }

    public static bool Stalemate_ShouldBeDetected()
    {
        GameEngine engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.Black, new Position(7, 7)), // h8
            new King(PieceColor.White, new Position(6, 5)), // f7
            new Queen(PieceColor.White, new Position(5, 6)) // g6
        );

        engine.SetCurrentTurnForTesting(PieceColor.Black);

        return DrawTestHelpers.ExpectState(
            engine.EvaluateGameStateForTesting(),
            GameState.Stalemate,
            "Stalemate position detected correctly."
        );
    }
}
