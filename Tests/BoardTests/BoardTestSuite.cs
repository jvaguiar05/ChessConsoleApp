namespace ChessConsoleApp.Tests.BoardTests;

public static class BoardTestSuite
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= BoardTests.RunAll();

        return allPassed;
    }
}
