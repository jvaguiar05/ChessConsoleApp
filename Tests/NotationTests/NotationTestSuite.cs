namespace ChessConsoleApp.Tests.NotationTests;

public static class NotationTestSuite
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= NotationTests.RunAll();

        return allPassed;
    }
}
