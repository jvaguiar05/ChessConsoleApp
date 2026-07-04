using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;
using static ChessConsoleApp.Tests.MoveTests.MoveTestHelpers;

namespace ChessConsoleApp.Tests.MoveTests;

public static class SelectionTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= MoveFromEmptySquare_ShouldBeRejected();
        allPassed &= MoveOpponentPiece_ShouldBeRejected();

        return allPassed;
    }

    public static bool MoveFromEmptySquare_ShouldBeRejected()
    {
        GameEngine engine = CreateEngine();

        GameState state = engine.TestMoveInput("e4 e5");

        bool allPassed = true;

        allPassed &= ExpectState(
            state,
            GameState.Running,
            "Move from empty square keeps game running."
        );

        allPassed &= ExpectEmptySquare(
            engine,
            new Position(3, 4), // e4
            "Empty source square remains empty."
        );

        return allPassed;
    }

    public static bool MoveOpponentPiece_ShouldBeRejected()
    {
        GameEngine engine = CreateEngine();

        GameState state = engine.TestMoveInput("e7 e5");

        bool allPassed = true;

        allPassed &= ExpectState(
            state,
            GameState.Running,
            "Moving opponent piece is rejected without ending the game."
        );

        allPassed &= ExpectPieceAt<Pawn>(
            engine,
            new Position(6, 4), // e7
            PieceColor.Black,
            "Opponent pawn remains on original square."
        );

        allPassed &= ExpectEmptySquare(
            engine,
            new Position(4, 4), // e5
            "Destination square remains empty after rejected move."
        );

        return allPassed;
    }
}
