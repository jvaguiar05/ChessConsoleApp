using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;

namespace ChessConsoleApp.Tests.DrawTests;

public static class FiftyMoveRuleTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= FiftyMoveRule_ShouldTriggerAt100HalfMoves();
        allPassed &= FiftyMoveRule_ShouldResetAfterPawnMove();
        allPassed &= FiftyMoveRule_ShouldResetAfterCapture();

        return allPassed;
    }

    public static bool FiftyMoveRule_ShouldTriggerAt100HalfMoves()
    {
        GameEngine engine = new GameEngine();

        engine.SetHalfMoveClockForTesting(99);

        GameState state = engine.TestMoveInput("b1 c3");

        return DrawTestHelpers.ExpectState(
            state,
            GameState.DrawByFiftyMoveRule,
            "Fifty-move rule triggered at 100 half-moves."
        );
    }

    public static bool FiftyMoveRule_ShouldResetAfterPawnMove()
    {
        GameEngine engine = new GameEngine();

        engine.SetHalfMoveClockForTesting(99);

        GameState state = engine.TestMoveInput("a2 a3");

        return DrawTestHelpers.ExpectNotState(
            state,
            GameState.DrawByFiftyMoveRule,
            "Pawn move resets fifty-move counter."
        );
    }

    public static bool FiftyMoveRule_ShouldResetAfterCapture()
    {
        GameEngine engine = new GameEngine();

        engine.SetHalfMoveClockForTesting(98);

        GameState setupState = DrawTestHelpers.PlayMoves(engine, "b1 c3", "d7 d5");

        if (
            !DrawTestHelpers.ExpectNotState(
                setupState,
                GameState.DrawByFiftyMoveRule,
                "Capture setup did not trigger fifty-move rule."
            )
        )
        {
            return false;
        }

        engine.SetHalfMoveClockForTesting(99);

        GameState captureState = engine.TestMoveInput("c3 d5");

        return DrawTestHelpers.ExpectNotState(
            captureState,
            GameState.DrawByFiftyMoveRule,
            "Capture resets fifty-move counter."
        );
    }
}
