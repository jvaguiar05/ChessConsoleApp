using ChessConsoleApp.Tests.BoardTests;
using ChessConsoleApp.Tests.DrawTests;
using ChessConsoleApp.Tests.GameStateTests;
using ChessConsoleApp.Tests.MoveTests;
using ChessConsoleApp.Tests.NotationTests;

namespace ChessConsoleApp.Tests;

public static class TestSuite
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= DrawTestSuite.RunAll();
        allPassed &= NotationTestSuite.RunAll();
        allPassed &= BoardTestSuite.RunAll();
        allPassed &= MoveTestSuite.RunAll();
        allPassed &= GameStateTestSuite.RunAll();

        return allPassed;
    }
}
