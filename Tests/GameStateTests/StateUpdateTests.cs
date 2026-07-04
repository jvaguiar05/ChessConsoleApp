using ChessGame.Enums;
using ChessGame.Models.Pieces;
using static ChessConsoleApp.Tests.GameStateTests.GameStateTestHelpers;

namespace ChessConsoleApp.Tests.GameStateTests;

public static class StateUpdateTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= LegalMove_ShouldKeepGameRunning();
        allPassed &= LegalMove_ShouldSwitchTurn();
        allPassed &= IllegalMove_ShouldNotSwitchTurn();
        allPassed &= LegalMove_ShouldUpdateBoard();
        allPassed &= Capture_ShouldUpdateBoard();

        return allPassed;
    }

    public static bool LegalMove_ShouldKeepGameRunning()
    {
        var engine = CreateEngine();

        GameState state = engine.TestMoveInput("e2 e4");

        return ExpectState(state, GameState.Running, "Legal move keeps game running.");
    }

    public static bool LegalMove_ShouldSwitchTurn()
    {
        var engine = CreateEngine();

        engine.TestMoveInput("e2 e4");

        GameState state = engine.TestMoveInput("e7 e5");

        bool passed = state == GameState.Running && HasPiece<Pawn>(engine, "e5", PieceColor.Black);

        return PrintResult(passed, "Legal move switches turn to opponent.");
    }

    public static bool IllegalMove_ShouldNotSwitchTurn()
    {
        var engine = CreateEngine();

        engine.TestMoveInput("e2 e5");

        GameState state = engine.TestMoveInput("e2 e4");

        bool passed = state == GameState.Running && HasPiece<Pawn>(engine, "e4", PieceColor.White);

        return PrintResult(passed, "Illegal move does not switch turn.");
    }

    public static bool LegalMove_ShouldUpdateBoard()
    {
        var engine = CreateEngine();

        engine.TestMoveInput("g1 f3");

        bool passed = HasPiece<Knight>(engine, "f3", PieceColor.White) && IsEmpty(engine, "g1");

        return PrintResult(passed, "Legal move updates board.");
    }

    public static bool Capture_ShouldUpdateBoard()
    {
        var engine = CreateEngine();

        engine.TestMoveInput("e2 e4");
        engine.TestMoveInput("d7 d5");
        engine.TestMoveInput("e4 d5");

        bool passed = HasPiece<Pawn>(engine, "d5", PieceColor.White) && IsEmpty(engine, "e4");

        return PrintResult(passed, "Capture updates board and removes captured piece.");
    }
}
