namespace ChessConsoleApp.Tests.EngineTests;

public static class EngineTestSuite
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= FenConverterTests.RunAll();

        return allPassed;
    }
}
