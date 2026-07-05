using ChessConsoleApp.Core;
using ChessConsoleApp.Core.Moves;
using ChessConsoleApp.Engine;
using ChessConsoleApp.Models;

namespace ChessConsoleApp.Tests.EngineTests;

public static class FenConverterTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= InitialPositionFen_ShouldBeCorrect();
        allPassed &= FenAfterWhiteDoublePawnMove_ShouldIncludeEnPassantTarget();
        allPassed &= FenAfterWhiteAndBlackPawnMoves_ShouldIncrementFullMoveNumber();

        return allPassed;
    }

    private static bool InitialPositionFen_ShouldBeCorrect()
    {
        GameSession session = CreateSession();

        string fen = FenConverter.ToFen(session);

        return AssertEqual(
            "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
            fen,
            nameof(InitialPositionFen_ShouldBeCorrect)
        );
    }

    private static bool FenAfterWhiteDoublePawnMove_ShouldIncludeEnPassantTarget()
    {
        GameSession session = CreateSession();

        ExecuteMove(session, "e2", "e4");

        string fen = FenConverter.ToFen(session);

        return AssertEqual(
            "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1",
            fen,
            nameof(FenAfterWhiteDoublePawnMove_ShouldIncludeEnPassantTarget)
        );
    }

    private static bool FenAfterWhiteAndBlackPawnMoves_ShouldIncrementFullMoveNumber()
    {
        GameSession session = CreateSession();

        ExecuteMove(session, "e2", "e4");
        ExecuteMove(session, "e7", "e5");

        string fen = FenConverter.ToFen(session);

        return AssertEqual(
            "rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2",
            fen,
            nameof(FenAfterWhiteAndBlackPawnMoves_ShouldIncrementFullMoveNumber)
        );
    }

    private static GameSession CreateSession()
    {
        var session = new GameSession();
        session.Initialize();

        return session;
    }

    private static void ExecuteMove(GameSession session, string from, string to)
    {
        var validator = new MoveValidator();
        var executor = new MoveExecutor(validator);

        MoveExecutionResult result = executor.Execute(
            session,
            Position.FromAlgebraic(from),
            Position.FromAlgebraic(to)
        );

        if (!result.Succeeded)
            throw new InvalidOperationException(result.ErrorMessage);

        session.SwitchTurn();
    }

    private static bool AssertEqual(string expected, string actual, string testName)
    {
        if (expected == actual)
        {
            Console.WriteLine($" [PASS] {testName}");
            return true;
        }

        Console.WriteLine($" [FAIL] {testName}");
        Console.WriteLine($" Expected: {expected}");
        Console.WriteLine($" Actual:   {actual}");

        return false;
    }
}
