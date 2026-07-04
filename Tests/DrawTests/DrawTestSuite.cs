namespace ChessConsoleApp.Tests.DrawTests;

public static class DrawTestSuite
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= RepetitionTests.RunAll();
        allPassed &= FiftyMoveRuleTests.RunAll();
        allPassed &= LackOfMaterialTests.RunAll();
        allPassed &= StalemateTests.RunAll();

        return allPassed;
    }
}
