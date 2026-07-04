using ChessGame.Core;
using ChessGame.Enums;
using ChessGame.Models;
using ChessGame.Models.Pieces;

namespace ChessConsoleApp.Tests.MoveTests;

public static class MoveTestHelpers
{
    public static GameEngine CreateEngine()
    {
        GameEngine engine = new GameEngine();

        engine.SuppressDiagnosticsForTesting();

        return engine;
    }

    public static GameEngine CreateEngineWith(params Piece[] pieces)
    {
        GameEngine engine = CreateEngine();

        engine.BoardForTesting.ClearForTesting();

        foreach (Piece piece in pieces)
            engine.BoardForTesting.PlacePieceForTesting(piece);

        return engine;
    }

    public static bool ExpectMoveState(
        GameEngine engine,
        string move,
        GameState expectedState,
        string passMessage
    )
    {
        GameState actualState = engine.TestMoveInput(move);

        return ExpectState(actualState, expectedState, passMessage);
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

    public static bool ExpectPieceAt<TPiece>(
        GameEngine engine,
        Position position,
        PieceColor color,
        string passMessage
    )
        where TPiece : Piece
    {
        bool passed = engine.BoardForTesting[position] is TPiece piece && piece.Color == color;

        Console.WriteLine(passed ? $" [PASS] {passMessage}" : $" [FAIL] {passMessage}");
        return passed;
    }

    public static bool ExpectPieceAt<TPiece>(
        GameEngine engine,
        string square,
        PieceColor color,
        string passMessage
    )
        where TPiece : Piece
    {
        return ExpectPieceAt<TPiece>(engine, Position.FromAlgebraic(square), color, passMessage);
    }

    public static bool ExpectEmptySquare(GameEngine engine, Position position, string passMessage)
    {
        bool passed = engine.BoardForTesting[position] == null;

        Console.WriteLine(passed ? $" [PASS] {passMessage}" : $" [FAIL] {passMessage}");
        return passed;
    }

    public static GameState PlayMoves(GameEngine engine, params string[] moves)
    {
        GameState state = GameState.Running;

        foreach (string move in moves)
            state = engine.TestMoveInput(move);

        return state;
    }

    public static bool ExpectLegalMove<TPiece>(
        GameEngine engine,
        string move,
        string destination,
        PieceColor color,
        string passMessage
    )
        where TPiece : Piece
    {
        engine.TestMoveInput(move);

        return ExpectPieceAt<TPiece>(engine, destination, color, passMessage);
    }

    public static bool ExpectIllegalMove<TPiece>(
        GameEngine engine,
        string move,
        string originalPosition,
        PieceColor color,
        string passMessage
    )
        where TPiece : Piece
    {
        engine.TestMoveInput(move);

        return ExpectPieceAt<TPiece>(engine, originalPosition, color, passMessage);
    }
}
