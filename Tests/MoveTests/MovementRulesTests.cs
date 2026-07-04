using ChessGame.Core;
using ChessGame.Enums;
using ChessGame.Models.Pieces;
using static ChessConsoleApp.Tests.MoveTests.MoveTestHelpers;

namespace ChessConsoleApp.Tests.MoveTests;

public static class MovementRulesTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= Pawn_ShouldMoveOneSquareForward();
        allPassed &= Pawn_ShouldMoveTwoSquaresFromStartingPosition();
        allPassed &= Pawn_ShouldNotMoveThreeSquares();
        allPassed &= Pawn_ShouldNotMoveSideways();
        allPassed &= Pawn_ShouldCaptureDiagonally();

        allPassed &= Knight_ShouldMoveInLShape();
        allPassed &= Knight_ShouldJumpOverPieces();
        allPassed &= Knight_ShouldNotMoveLikeBishop();

        allPassed &= Bishop_ShouldMoveDiagonally();
        allPassed &= Bishop_ShouldNotMoveStraight();

        allPassed &= Rook_ShouldMoveVertically();
        allPassed &= Rook_ShouldMoveHorizontally();
        allPassed &= Rook_ShouldNotMoveDiagonally();

        allPassed &= Queen_ShouldMoveDiagonally();
        allPassed &= Queen_ShouldMoveStraight();

        allPassed &= King_ShouldMoveOneSquare();
        allPassed &= King_ShouldNotMoveTwoSquaresNormally();

        return allPassed;
    }

    // ---------- PAWN ----------

    public static bool Pawn_ShouldMoveOneSquareForward()
    {
        GameEngine engine = CreateEngine();

        return ExpectLegalMove<Pawn>(
            engine,
            "e2 e3",
            "e3",
            PieceColor.White,
            "Pawn moves one square forward."
        );
    }

    public static bool Pawn_ShouldMoveTwoSquaresFromStartingPosition()
    {
        GameEngine engine = CreateEngine();

        return ExpectLegalMove<Pawn>(
            engine,
            "e2 e4",
            "e4",
            PieceColor.White,
            "Pawn moves two squares from starting position."
        );
    }

    public static bool Pawn_ShouldNotMoveThreeSquares()
    {
        GameEngine engine = CreateEngine();

        return ExpectIllegalMove<Pawn>(
            engine,
            "e2 e5",
            "e2",
            PieceColor.White,
            "Pawn cannot move three squares."
        );
    }

    public static bool Pawn_ShouldNotMoveSideways()
    {
        GameEngine engine = CreateEngine();

        return ExpectIllegalMove<Pawn>(
            engine,
            "e2 f2",
            "e2",
            PieceColor.White,
            "Pawn cannot move sideways."
        );
    }

    public static bool Pawn_ShouldCaptureDiagonally()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "e2 e4", "d7 d5");

        return ExpectLegalMove<Pawn>(
            engine,
            "e4 d5",
            "d5",
            PieceColor.White,
            "Pawn captures diagonally."
        );
    }

    // ---------- KNIGHT ----------

    public static bool Knight_ShouldMoveInLShape()
    {
        GameEngine engine = CreateEngine();

        return ExpectLegalMove<Knight>(
            engine,
            "b1 c3",
            "c3",
            PieceColor.White,
            "Knight moves in L-shape."
        );
    }

    public static bool Knight_ShouldJumpOverPieces()
    {
        GameEngine engine = CreateEngine();

        return ExpectLegalMove<Knight>(
            engine,
            "g1 f3",
            "f3",
            PieceColor.White,
            "Knight jumps over pieces."
        );
    }

    public static bool Knight_ShouldNotMoveLikeBishop()
    {
        GameEngine engine = CreateEngine();

        return ExpectIllegalMove<Knight>(
            engine,
            "b1 c2",
            "b1",
            PieceColor.White,
            "Knight cannot move like a bishop."
        );
    }

    // ---------- BISHOP ----------

    public static bool Bishop_ShouldMoveDiagonally()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "e2 e4", "a7 a6");

        return ExpectLegalMove<Bishop>(
            engine,
            "f1 b5",
            "b5",
            PieceColor.White,
            "Bishop moves diagonally."
        );
    }

    public static bool Bishop_ShouldNotMoveStraight()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "e2 e4", "a7 a6");

        return ExpectIllegalMove<Bishop>(
            engine,
            "f1 f3",
            "f1",
            PieceColor.White,
            "Bishop cannot move straight."
        );
    }

    // ---------- ROOK ----------

    public static bool Rook_ShouldMoveVertically()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "a2 a4", "h7 h6");

        return ExpectLegalMove<Rook>(
            engine,
            "a1 a3",
            "a3",
            PieceColor.White,
            "Rook moves vertically."
        );
    }

    public static bool Rook_ShouldMoveHorizontally()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "a2 a4", "h7 h6", "a1 a3", "h6 h5");

        return ExpectLegalMove<Rook>(
            engine,
            "a3 h3",
            "h3",
            PieceColor.White,
            "Rook moves horizontally."
        );
    }

    public static bool Rook_ShouldNotMoveDiagonally()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "a2 a4", "h7 h6");

        return ExpectIllegalMove<Rook>(
            engine,
            "a1 b2",
            "a1",
            PieceColor.White,
            "Rook cannot move diagonally."
        );
    }

    // ---------- QUEEN ----------

    public static bool Queen_ShouldMoveDiagonally()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "e2 e4", "a7 a6");

        return ExpectLegalMove<Queen>(
            engine,
            "d1 h5",
            "h5",
            PieceColor.White,
            "Queen moves diagonally."
        );
    }

    public static bool Queen_ShouldMoveStraight()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "d2 d4", "a7 a6");

        return ExpectLegalMove<Queen>(
            engine,
            "d1 d3",
            "d3",
            PieceColor.White,
            "Queen moves straight."
        );
    }

    // ---------- KING ----------

    public static bool King_ShouldMoveOneSquare()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "e2 e4", "a7 a6");

        return ExpectLegalMove<King>(
            engine,
            "e1 e2",
            "e2",
            PieceColor.White,
            "King moves one square."
        );
    }

    public static bool King_ShouldNotMoveTwoSquaresNormally()
    {
        GameEngine engine = CreateEngine();

        PlayMoves(engine, "e2 e4", "a7 a6");

        return ExpectIllegalMove<King>(
            engine,
            "e1 e3",
            "e1",
            PieceColor.White,
            "King cannot move two squares normally."
        );
    }
}
