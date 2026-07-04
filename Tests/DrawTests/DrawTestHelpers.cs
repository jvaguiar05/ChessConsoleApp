using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Tests.DrawTests;

public static class DrawTestHelpers
{
    public static GameEngine CreateEngineWith(params Piece[] pieces)
    {
        GameEngine engine = new GameEngine();

        engine.BoardForTesting.ClearForTesting();

        foreach (Piece piece in pieces)
            engine.BoardForTesting.PlacePieceForTesting(piece);

        return engine;
    }

    public static GameState PlayMoves(GameEngine engine, params string[] moves)
    {
        GameState state = GameState.Running;

        foreach (string move in moves)
            state = engine.TestMoveInput(move);

        return state;
    }

    public static bool ExpectState(GameState actual, GameState expected, string passMessage)
    {
        if (actual == expected)
        {
            Console.WriteLine($" [PASS] {passMessage}");
            return true;
        }

        Console.WriteLine($" [FAIL] Expected {expected}, got {actual}.");
        return false;
    }

    public static bool ExpectNotState(GameState actual, GameState forbidden, string passMessage)
    {
        if (actual != forbidden)
        {
            Console.WriteLine($" [PASS] {passMessage}");
            return true;
        }

        Console.WriteLine($" [FAIL] Unexpectedly got {forbidden}.");
        return false;
    }

    public static bool ExpectStateAfterMoves(
        GameState expected,
        string passMessage,
        params string[] moves
    )
    {
        GameEngine engine = new GameEngine();
        GameState actual = PlayMoves(engine, moves);

        return ExpectState(actual, expected, passMessage);
    }

    public static bool ExpectNotStateAfterMoves(
        GameState forbidden,
        string passMessage,
        params string[] moves
    )
    {
        GameEngine engine = new GameEngine();
        GameState actual = PlayMoves(engine, moves);

        return ExpectNotState(actual, forbidden, passMessage);
    }
}
