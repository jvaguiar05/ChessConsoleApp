using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;
using static ChessConsoleApp.Tests.MoveTests.MoveTestHelpers;

namespace ChessConsoleApp.Tests.MoveTests;

public static class KingSafetyTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= King_ShouldNotMoveIntoCheck();
        allPassed &= Player_ShouldNotExposeOwnKingToCheck();
        allPassed &= Player_ShouldBeAllowedToBlockCheck();

        return allPassed;
    }

    public static bool King_ShouldNotMoveIntoCheck()
    {
        GameEngine engine = CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)), // e1
            new King(PieceColor.Black, new Position(7, 4)), // e8
            new Rook(PieceColor.Black, new Position(1, 0)) // a2 controls e2
        );

        engine.SetCurrentTurnForTesting(PieceColor.White);

        return ExpectIllegalMove<King>(
            engine,
            "e1 e2",
            "e1",
            PieceColor.White,
            "King cannot move into check."
        );
    }

    public static bool Player_ShouldNotExposeOwnKingToCheck()
    {
        GameEngine engine = CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)), // e1
            new Rook(PieceColor.White, new Position(1, 4)), // e2 shields king
            new King(PieceColor.Black, new Position(7, 7)), // h8
            new Rook(PieceColor.Black, new Position(7, 4)) // e8 attacks e-file
        );

        engine.SetCurrentTurnForTesting(PieceColor.White);

        return ExpectIllegalMove<Rook>(
            engine,
            "e2 f2",
            "e2",
            PieceColor.White,
            "Player cannot expose own king to check."
        );
    }

    public static bool Player_ShouldBeAllowedToBlockCheck()
    {
        GameEngine engine = CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)), // e1
            new Bishop(PieceColor.White, new Position(0, 5)), // f1
            new King(PieceColor.Black, new Position(7, 7)), // h8
            new Rook(PieceColor.Black, new Position(7, 4)) // e8 checks e-file
        );

        engine.SetCurrentTurnForTesting(PieceColor.White);

        return ExpectLegalMove<Bishop>(
            engine,
            "f1 e2",
            "e2",
            PieceColor.White,
            "Player can block check."
        );
    }
}
