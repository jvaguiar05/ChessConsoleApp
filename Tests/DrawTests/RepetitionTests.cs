using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;

namespace ChessConsoleApp.Tests.DrawTests;

public static class RepetitionTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= ThreefoldRepetition_ShouldTrigger();
        allPassed &= ThreefoldRepetition_ShouldNotTriggerEarly();
        allPassed &= ThreefoldRepetition_ShouldTriggerOnlyOnThirdOccurrence();

        return allPassed;
    }

    public static bool ThreefoldRepetition_ShouldTrigger()
    {
        return DrawTestHelpers.ExpectStateAfterMoves(
            GameState.DrawByRepetition,
            "Threefold repetition detected.",
            "b1 c3",
            "b8 c6",
            "c3 b1",
            "c6 b8",
            "b1 c3",
            "b8 c6",
            "c3 b1",
            "c6 b8"
        );
    }

    public static bool ThreefoldRepetition_ShouldNotTriggerEarly()
    {
        return DrawTestHelpers.ExpectNotStateAfterMoves(
            GameState.DrawByRepetition,
            "No early repetition detected.",
            "b1 c3",
            "b8 c6",
            "c3 b1",
            "c6 b8"
        );
    }

    public static bool ThreefoldRepetition_ShouldTriggerOnlyOnThirdOccurrence()
    {
        GameEngine engine = new GameEngine();

        GameState stateBeforeThirdOccurrence = DrawTestHelpers.PlayMoves(
            engine,
            "b1 c3",
            "b8 c6",
            "c3 b1",
            "c6 b8",
            "b1 c3",
            "b8 c6",
            "c3 b1"
        );

        if (
            !DrawTestHelpers.ExpectNotState(
                stateBeforeThirdOccurrence,
                GameState.DrawByRepetition,
                "Repetition did not trigger before third occurrence."
            )
        )
        {
            return false;
        }

        GameState finalState = engine.TestMoveInput("c6 b8");

        return DrawTestHelpers.ExpectState(
            finalState,
            GameState.DrawByRepetition,
            "Repetition triggered exactly on third occurrence."
        );
    }
}
