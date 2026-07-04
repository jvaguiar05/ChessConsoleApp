using ChessGame.Core;
using ChessGame.Enums;
using ChessGame.Models;
using ChessGame.Models.Pieces;

namespace ChessConsoleApp.Tests.GameStateTests;

public static class GameStateTestHelpers
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

    public static bool PrintResult(bool passed, string message)
    {
        Console.WriteLine(passed ? $" [PASS] {message}" : $" [FAIL] {message}");
        return passed;
    }

    public static bool HasPiece<TPiece>(GameEngine engine, string square, PieceColor color)
        where TPiece : Piece
    {
        Position position = Position.FromAlgebraic(square);

        return engine.BoardForTesting[position] is TPiece piece && piece.Color == color;
    }

    public static bool IsEmpty(GameEngine engine, string square)
    {
        return engine.BoardForTesting[Position.FromAlgebraic(square)] == null;
    }
}
