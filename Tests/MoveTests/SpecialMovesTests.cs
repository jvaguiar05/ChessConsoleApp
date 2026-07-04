using ChessGame.Core;
using ChessGame.Enums;
using ChessGame.Models;
using ChessGame.Models.Pieces;
using static ChessConsoleApp.Tests.MoveTests.MoveTestHelpers;

namespace ChessConsoleApp.Tests.MoveTests;

public static class SpecialMovesTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= Castling_Kingside_ShouldMoveKingAndRook();
        allPassed &= Castling_Queenside_ShouldMoveKingAndRook();

        allPassed &= EnPassant_ShouldCapturePawn();

        allPassed &= Promotion_ShouldReplacePawnWithQueen();

        return allPassed;
    }

    public static bool Castling_Kingside_ShouldMoveKingAndRook()
    {
        GameEngine engine = CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)), // e1
            new Rook(PieceColor.White, new Position(0, 7)), // h1
            new King(PieceColor.Black, new Position(7, 4)) // e8
        );

        engine.SetCurrentTurnForTesting(PieceColor.White);

        engine.TestMoveInput("e1 g1");

        bool allPassed = true;

        allPassed &= ExpectPieceAt<King>(
            engine,
            "g1",
            PieceColor.White,
            "Kingside castling moves king to g1."
        );

        allPassed &= ExpectPieceAt<Rook>(
            engine,
            "f1",
            PieceColor.White,
            "Kingside castling moves rook to f1."
        );

        return allPassed;
    }

    public static bool Castling_Queenside_ShouldMoveKingAndRook()
    {
        GameEngine engine = CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)), // e1
            new Rook(PieceColor.White, new Position(0, 0)), // a1
            new King(PieceColor.Black, new Position(7, 4)) // e8
        );

        engine.SetCurrentTurnForTesting(PieceColor.White);

        engine.TestMoveInput("e1 c1");

        bool allPassed = true;

        allPassed &= ExpectPieceAt<King>(
            engine,
            "c1",
            PieceColor.White,
            "Queenside castling moves king to c1."
        );

        allPassed &= ExpectPieceAt<Rook>(
            engine,
            "d1",
            PieceColor.White,
            "Queenside castling moves rook to d1."
        );

        return allPassed;
    }

    public static bool EnPassant_ShouldCapturePawn()
    {
        GameEngine engine = CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)), // e1
            new Pawn(PieceColor.White, new Position(4, 4)), // e5
            new King(PieceColor.Black, new Position(7, 4)), // e8
            new Pawn(PieceColor.Black, new Position(6, 3)) // d7
        );

        engine.SetCurrentTurnForTesting(PieceColor.Black);

        engine.TestMoveInput("d7 d5");
        engine.TestMoveInput("e5 d6");

        bool allPassed = true;

        allPassed &= ExpectPieceAt<Pawn>(
            engine,
            "d6",
            PieceColor.White,
            "En passant moves capturing pawn to d6."
        );

        allPassed &= ExpectEmptySquare(
            engine,
            new Position(4, 3), // d5
            "En passant removes captured pawn from d5."
        );

        return allPassed;
    }

    public static bool Promotion_ShouldReplacePawnWithQueen()
    {
        GameEngine engine = CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)), // e1
            new Pawn(PieceColor.White, new Position(6, 0)), // a7
            new King(PieceColor.Black, new Position(7, 4)) // e8
        );

        engine.SetCurrentTurnForTesting(PieceColor.White);
        engine.SetPromotionChoiceForTesting('Q');

        engine.TestMoveInput("a7 a8");

        return ExpectPieceAt<Queen>(
            engine,
            "a8",
            PieceColor.White,
            "Promotion replaces pawn with queen."
        );
    }
}
