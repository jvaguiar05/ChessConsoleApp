namespace ChessConsoleApp.Tests.MoveTests;

public static class MoveTestSuite
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= SelectionTests.RunAll();
        allPassed &= TurnOrderTests.RunAll();
        allPassed &= MovementRulesTests.RunAll();
        allPassed &= KingSafetyTests.RunAll();
        allPassed &= SpecialMovesTests.RunAll();

        return allPassed;
    }
}
