namespace ChessConsoleApp.Tests.GameStateTests;

public static class GameStateTestSuite
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= StateUpdateTests.RunAll();
        allPassed &= StateTransitionTests.RunAll();

        return allPassed;
    }
}
