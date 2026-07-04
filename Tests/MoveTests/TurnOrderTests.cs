using ChessGame.Core;
using ChessGame.Enums;
using ChessGame.Models;
using ChessGame.Models.Pieces;
using static ChessConsoleApp.Tests.MoveTests.MoveTestHelpers;

namespace ChessConsoleApp.Tests.MoveTests;

public static class TurnOrderTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= White_ShouldMoveFirst();
        allPassed &= Turn_ShouldSwitchAfterLegalMove();
        allPassed &= Turn_ShouldNotSwitchAfterIllegalMove();

        return allPassed;
    }

    public static bool White_ShouldMoveFirst()
    {
        GameEngine engine = CreateEngine();

        GameState state = engine.TestMoveInput("e7 e5");

        bool allPassed = true;

        allPassed &= ExpectState(state, GameState.Running, "Black cannot move first.");

        allPassed &= ExpectPieceAt<Pawn>(
            engine,
            new Position(6, 4), // e7
            PieceColor.Black,
            "Black pawn remains on e7."
        );

        allPassed &= ExpectEmptySquare(
            engine,
            new Position(4, 4), // e5
            "e5 remains empty after rejected black move."
        );

        return allPassed;
    }

    public static bool Turn_ShouldSwitchAfterLegalMove()
    {
        GameEngine engine = CreateEngine();

        engine.TestMoveInput("e2 e4");

        GameState state = engine.TestMoveInput("e7 e5");

        bool allPassed = true;

        allPassed &= ExpectState(
            state,
            GameState.Running,
            "Black can move after White makes a legal move."
        );

        allPassed &= ExpectPieceAt<Pawn>(
            engine,
            new Position(4, 4), // e5
            PieceColor.Black,
            "Black pawn moved after turn switched."
        );

        return allPassed;
    }

    public static bool Turn_ShouldNotSwitchAfterIllegalMove()
    {
        GameEngine engine = CreateEngine();

        engine.TestMoveInput("e2 e5"); // illegal pawn move, White should still be on turn

        GameState state = engine.TestMoveInput("e2 e4");

        bool allPassed = true;

        allPassed &= ExpectState(
            state,
            GameState.Running,
            "White can still move after illegal move attempt."
        );

        allPassed &= ExpectPieceAt<Pawn>(
            engine,
            new Position(3, 4), // e4
            PieceColor.White,
            "White pawn moves after illegal attempt because turn did not switch."
        );

        return allPassed;
    }
}
